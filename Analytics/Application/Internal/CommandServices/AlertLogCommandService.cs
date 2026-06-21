using Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.Enums;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Analytics.Domain.Repositories;
using Hampcoders.Electrolink.API.Analytics.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using MediatR;
using ServiceRequestId = Hampcoders.Electrolink.API.Analytics.Domain.Model.ValueObjects.ServiceRequestId;

namespace Hampcoders.Electrolink.API.Analytics.Application.Internal.CommandServices;

public class AlertLogCommandService(
    IAlertLogRepository alertLogRepository,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<AlertLogCommandService> logger)
    : IAlertLogCommandService
{
    private async Task<AlertLog> GetOrCreateAlertLogAsync(string homeownerId)
    {
        var log = await alertLogRepository.FindByOwnerAsync(ClientIdentity.FromOwnerId(homeownerId));
        if (log != null) return log;

        log = AlertLog.CreateFor(ClientIdentity.FromOwnerId(homeownerId));
        await alertLogRepository.AddAsync(log);
        return log;
    }

    public async Task RecordAnomalyAlertAsync(
        string homeownerId,
        string anomalyEventId,
        string severity,
        string? circuitId)
    {
        var log = await GetOrCreateAlertLogAsync(homeownerId);
        try
        {
            log.RecordAlert(
                SourceEventId.From(anomalyEventId),
                AlertSourceBC.IoTMonitoring,
                AlertType.AnomalyDetected,
                Enum.Parse<AlertSeverity>(severity, ignoreCase: true),
                circuitId);

            alertLogRepository.Update(log);
            await unitOfWork.CompleteAsync();

            foreach (var domainEvent in log.DomainEvents)
                await mediator.Publish(domainEvent, CancellationToken.None);
            log.ClearDomainEvents();
        }
        catch (InvalidOperationException)
        {
            // Idempotencia
        }
    }

    public async Task RecordThresholdAlertAsync(
        string homeownerId,
        string circuitId,
        decimal consumedKWh,
        string readingEventId)
    {
        var log = await GetOrCreateAlertLogAsync(homeownerId);

        var recentThresholdAlert = log.Entries.Any(e =>
            e.CircuitId == circuitId &&
            e.AlertType == AlertType.ThresholdExceeded &&
            e.Status == AlertStatus.Active &&
            e.TriggeredAt >= DateTime.UtcNow.AddHours(-1));

        if (recentThresholdAlert)
            return;

        try
        {
            log.RecordAlert(
                SourceEventId.From(readingEventId),
                AlertSourceBC.Analytics,
                AlertType.ThresholdExceeded,
                AlertSeverity.Medium,
                circuitId);

            alertLogRepository.Update(log);
            await unitOfWork.CompleteAsync();

            foreach (var domainEvent in log.DomainEvents)
                await mediator.Publish(domainEvent, CancellationToken.None);
            log.ClearDomainEvents();
        }
        catch (InvalidOperationException) { }
    }

    public async Task RecordDeviceDisconnectionAlertAsync(
        string homeownerId,
        string deviceEventId,
        string? circuitId)
    {
        var log = await GetOrCreateAlertLogAsync(homeownerId);
        try
        {
            log.RecordAlert(
                SourceEventId.From(deviceEventId),
                AlertSourceBC.IoTMonitoring,
                AlertType.DeviceDisconnected,
                AlertSeverity.High,
                circuitId);

            alertLogRepository.Update(log);
            await unitOfWork.CompleteAsync();

            foreach (var domainEvent in log.DomainEvents)
                await mediator.Publish(domainEvent, CancellationToken.None);
            log.ClearDomainEvents();
        }
        catch (InvalidOperationException) { }
    }

    public async Task RecordCircuitToggleAlertAsync(
        string homeownerId,
        string relayEventId,
        string circuitId)
    {
        var log = await GetOrCreateAlertLogAsync(homeownerId);
        try
        {
            log.RecordAlert(
                SourceEventId.From(relayEventId),
                AlertSourceBC.IoTMonitoring,
                AlertType.CircuitToggled,
                AlertSeverity.Low,
                circuitId);

            alertLogRepository.Update(log);
            await unitOfWork.CompleteAsync();

            foreach (var domainEvent in log.DomainEvents)
                await mediator.Publish(domainEvent, CancellationToken.None);
            log.ClearDomainEvents();
        }
        catch (InvalidOperationException) { }
    }

    public async Task ResolveAlertAsync(string homeownerId, string sourceEventId)
    {
        var log = await alertLogRepository.FindByOwnerAsync(ClientIdentity.FromOwnerId(homeownerId))
            ?? throw new InvalidOperationException($"No alert log found for homeowner '{homeownerId}'.");

        log.ResolveAlertBySourceEvent(SourceEventId.From(sourceEventId));

        alertLogRepository.Update(log);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in log.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        log.ClearDomainEvents();
    }

    public async Task AcknowledgeAlertAsync(string homeownerId, string entryId)
    {
        var log = await alertLogRepository.FindByOwnerAsync(ClientIdentity.FromOwnerId(homeownerId))
            ?? throw new InvalidOperationException($"No alert log found for homeowner '{homeownerId}'.");

        log.AcknowledgeAlert(AlertEntryId.From(entryId));

        alertLogRepository.Update(log);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in log.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        log.ClearDomainEvents();
    }

    public async Task LinkAlertToServiceRequestAsync(
        string homeownerId,
        string entryId,
        string serviceRequestId)
    {
        var log = await alertLogRepository.FindByOwnerAsync(ClientIdentity.FromOwnerId(homeownerId))
            ?? throw new InvalidOperationException($"No alert log found for homeowner '{homeownerId}'.");

        log.LinkAlertToServiceRequest(
            AlertEntryId.From(entryId),
            ServiceRequestId.From(serviceRequestId));

        alertLogRepository.Update(log);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in log.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        log.ClearDomainEvents();
    }
}
