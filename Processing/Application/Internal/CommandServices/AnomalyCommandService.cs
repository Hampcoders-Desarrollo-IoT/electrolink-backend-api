using Hampcoders.Electrolink.API.Processing.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Processing.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Processing.Domain.Model.Entities;
using Hampcoders.Electrolink.API.Processing.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Processing.Domain.Repositories;
using Hampcoders.Electrolink.API.Processing.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using MediatR;

namespace Hampcoders.Electrolink.API.Processing.Application.Internal.CommandServices;

public class AnomalyCommandService(
    IDeviceReadingStreamRepository streamRepository,
    IAnomalyRecordRepository       anomalyRepository,
    IRelayCommandService           relayService,
    IUnitOfWork                    unitOfWork,
    IMediator                      mediator,
    ILogger<AnomalyCommandService> logger)
    : IAnomalyCommandService
{
    private const int AutoResolveThreshold = 5;

    public async Task Handle(EvaluateReadingForAnomaliesCommand command)
    {
        var stream = await streamRepository.FindByStreamIdAsync(command.StreamId);
        if (stream is null) return;

        var latestReading = stream.Readings
            .OrderByDescending(r => r.Timestamp)
            .FirstOrDefault(r => r.ReadingId.Value == command.ReadingId);

        if (latestReading is null) return;

        var thresholds = stream.CustomThresholds;
        var detected   = DetectAnomaly(latestReading, stream.Readings, thresholds);

        if (detected is not null)
        {
            var existing = await anomalyRepository.FindActiveByDeviceAndTypeAsync(
                stream.DeviceId.Value, detected.Value.type);

            if (existing is not null)
            {
                existing.Supersede();
                await anomalyRepository.UpdateAsync(existing);
                stream.RegisterAnomalyDetection();
            }

            var anomaly = AnomalyRecord.Detect(
                stream.DeviceId,
                stream.PropertyId,
                stream.Owner,
                detected.Value.type,
                detected.Value.severity,
                ReadingId.From(command.ReadingId));

            await anomalyRepository.AddAsync(anomaly);
            stream.ResetNormalCount();
            stream.RegisterAnomalyDetection();

            await unitOfWork.CompleteAsync();

            if (detected.Value.severity == EAnomalySeverity.Critical
                && detected.Value.type == EAnomalyType.ShortCircuitRisk)
            {
                await Handle(new AutoIssueRelayCommandCommand(
                    stream.DeviceId.Value,
                    stream.PropertyId.Value,
                    anomaly.AnomalyId.Value));
            }

            foreach (var ev in anomaly.DomainEvents)
                await mediator.Publish(ev, CancellationToken.None);
            anomaly.ClearDomainEvents();
        }
        else
        {
            stream.RegisterNormalReading();

            if (stream.ShouldTryAutoResolveAnomalies(AutoResolveThreshold))
            {
                var activeAnomalies = await anomalyRepository.FindActiveByPropertyAsync(
                    stream.PropertyId.Value);

                foreach (var anomaly in activeAnomalies.Where(
                    a => a.Severity != EAnomalySeverity.Critical && a.DeviceId == stream.DeviceId))
                {
                    anomaly.ResolveAutomatically();
                    await anomalyRepository.UpdateAsync(anomaly);

                    foreach (var ev in anomaly.DomainEvents)
                        await mediator.Publish(ev, CancellationToken.None);
                    anomaly.ClearDomainEvents();
                }
            }

            await streamRepository.UpdateAsync(stream);
            await unitOfWork.CompleteAsync();
        }
    }

    private static (EAnomalyType type, EAnomalySeverity severity)?
        DetectAnomaly(
            Reading latest,
            IReadOnlyCollection<Reading> window,
            ThresholdConfig thresholds)
    {
        float nomV   = thresholds.NominalVoltage;
        float nomHz  = thresholds.NominalFrequency;

        if (latest.Current > thresholds.MaxCurrentAmps && latest.PowerFactor < 0.7f)
            return (EAnomalyType.ShortCircuitRisk, EAnomalySeverity.Critical);

        if (latest.Voltage > nomV * 1.20f)
            return (EAnomalyType.VoltageSpike, EAnomalySeverity.High);

        var last3 = window.OrderByDescending(r => r.Timestamp).Take(3).ToList();
        if (last3.Count == 3 && last3.All(r => r.Voltage < nomV * 0.85f))
            return (EAnomalyType.VoltageSag, EAnomalySeverity.Medium);

        var windowKw = window
            .Where(r => r.Timestamp >= DateTime.UtcNow.AddMinutes(-30))
            .Average(r => (double)r.ActivePowerKw);
        if (windowKw * 1000 > thresholds.MaxConsumptionWatts)
        {
            var severity = windowKw * 1000 > thresholds.MaxConsumptionWatts * 1.5f
                ? EAnomalySeverity.High : EAnomalySeverity.Medium;
            return (EAnomalyType.SustainedOverconsumption, severity);
        }

        var last5 = window.OrderByDescending(r => r.Timestamp).Take(5).ToList();
        if (last5.Count == 5 && last5.All(r => r.PowerFactor < thresholds.MinPowerFactor))
            return (EAnomalyType.PowerFactorDegradation, EAnomalySeverity.Low);

        if (last3.Count == 3 && last3.All(r => MathF.Abs(r.Frequency - nomHz) > 1f))
            return (EAnomalyType.AbnormalFrequency, EAnomalySeverity.Medium);

        return null;
    }

    public async Task Handle(AutoIssueRelayCommandCommand command)
    {
        logger.LogWarning(
            "[IoT] CriticalAnomalyAutoProtection — Device: {DeviceId} | Anomaly: {AnomalyId}",
            command.DeviceId, command.AnomalyId);

        await relayService.Handle(new IssueRelayCommandCommand(
            command.DeviceId,
            command.PropertyId,
            "SYSTEM",
            null!,
            "Open"));
    }

    public async Task Handle(AcknowledgeAnomalyCommand command)
    {
        var anomaly = await anomalyRepository.FindByIdAsync(
            AnomalyRecordId.From(command.AnomalyId))
            ?? throw new KeyNotFoundException($"Anomaly {command.AnomalyId} not found.");

        anomaly.Acknowledge();
        await anomalyRepository.UpdateAsync(anomaly);
        await unitOfWork.CompleteAsync();

        foreach (var ev in anomaly.DomainEvents)
            await mediator.Publish(ev, CancellationToken.None);
        anomaly.ClearDomainEvents();
    }

    public async Task Handle(ForceResolveAnomalyCommand command)
    {
        var anomaly = await anomalyRepository.FindByIdAsync(
            AnomalyRecordId.From(command.AnomalyId))
            ?? throw new KeyNotFoundException($"Anomaly {command.AnomalyId} not found.");

        anomaly.ForceResolve(command.ResolvedByActorId);
        await anomalyRepository.UpdateAsync(anomaly);
        await unitOfWork.CompleteAsync();

        foreach (var ev in anomaly.DomainEvents)
            await mediator.Publish(ev, CancellationToken.None);
        anomaly.ClearDomainEvents();
    }

    public async Task Handle(ResolveAnomalyCommand command)
    {
        if (command.ConsecutiveNormalReadings < AutoResolveThreshold) return;

        var anomaly = await anomalyRepository.FindByIdAsync(
            AnomalyRecordId.From(command.AnomalyId));

        if (anomaly is null || anomaly.Severity == EAnomalySeverity.Critical) return;

        anomaly.ResolveAutomatically();
        await anomalyRepository.UpdateAsync(anomaly);
        await unitOfWork.CompleteAsync();

        foreach (var ev in anomaly.DomainEvents)
            await mediator.Publish(ev, CancellationToken.None);
        anomaly.ClearDomainEvents();
    }
}
