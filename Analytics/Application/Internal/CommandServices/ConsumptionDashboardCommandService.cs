using Hampcoders.Electrolink.API.Analytics.Application.Internal.Services;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.Enums;
using Hampcoders.Electrolink.API.Analytics.Domain.Repositories;
using Hampcoders.Electrolink.API.Analytics.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using MediatR;

namespace Hampcoders.Electrolink.API.Analytics.Application.Internal.CommandServices;

public class ConsumptionDashboardCommandService(
    IConsumptionDashboardRepository dashboardRepository,
    IDashboardProjectionService projectionService,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<ConsumptionDashboardCommandService> logger)
    : IConsumptionDashboardCommandService
{
    public async Task InitializeDashboardAsync(
        string homeownerId,
        string propertyId,
        List<string> deviceIds,
        string planTier)
    {
        var existing = await dashboardRepository.FindByOwnerAsync(ClientIdentity.FromHomeowner(homeownerId));
        if (existing != null)
            return;

        var plan = Enum.Parse<PlanTier>(planTier, ignoreCase: true);

        var dashboard = ConsumptionDashboard.Initialize(
            ClientIdentity.FromHomeowner(homeownerId),
            PropertyId.From(propertyId),
            deviceIds.Select(DeviceId.From).ToList(),
            plan);

        await dashboardRepository.AddAsync(dashboard);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in dashboard.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        dashboard.ClearDomainEvents();

        logger.LogInformation("[Analytics BC] Dashboard initialized for homeowner {HomeownerId}", homeownerId);
    }

    public async Task UpdateConsumptionDashboardAsync(
        string homeownerId,
        string deviceId,
        string circuitId,
        decimal kWh,
        decimal voltage,
        decimal current,
        DateTime readingTimestamp)
    {
        var dashboard = await dashboardRepository.FindByOwnerAsync(ClientIdentity.FromHomeowner(homeownerId))
            ?? throw new InvalidOperationException($"No dashboard found for homeowner '{homeownerId}'.");

        var (normalizedTs, granularity) = dashboard.ResolveReadingTimestamp(readingTimestamp);

        if (!dashboard.ApplyReading(
                DeviceId.From(deviceId),
                circuitId,
                kWh,
                readingTimestamp))
            return;

        await projectionService.RecordTimeSeriesAsync(
            dashboard.DashboardId.Value,
            normalizedTs,
            kWh,
            granularity.ToString());

        if (dashboard.PlanTier is PlanTier.EnterpriseBasic or PlanTier.EnterprisePremium)
        {
            await projectionService.UpsertCircuitSummaryAsync(
                dashboard.DashboardId.Value,
                circuitId,
                kWh,
                voltage,
                current,
                readingTimestamp);
        }

        dashboardRepository.Update(dashboard);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in dashboard.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        dashboard.ClearDomainEvents();
    }

    public async Task UpgradeDashboardTierAsync(string homeownerId, string newPlanTier)
    {
        var plan = Enum.Parse<PlanTier>(newPlanTier, ignoreCase: true);

        var dashboard = await dashboardRepository.FindByOwnerAsync(ClientIdentity.FromHomeowner(homeownerId))
            ?? throw new InvalidOperationException($"No dashboard found for homeowner '{homeownerId}'.");

        dashboard.UpgradeTier(plan);
        dashboardRepository.Update(dashboard);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in dashboard.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        dashboard.ClearDomainEvents();
    }

    public async Task GenerateCostProjectionAsync(string homeownerId, decimal electricityRatePerKWh)
    {
        var dashboard = await dashboardRepository.FindByOwnerAsync(ClientIdentity.FromHomeowner(homeownerId))
            ?? throw new InvalidOperationException($"No dashboard found for homeowner '{homeownerId}'.");

        var now = DateTime.UtcNow;
        var accumulated = await projectionService.GetAccumulatedKWhForMonthAsync(
            dashboard.DashboardId.Value, now.Year, now.Month);

        dashboard.UpdateCostProjection(electricityRatePerKWh, accumulated);
        dashboardRepository.Update(dashboard);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in dashboard.DomainEvents)
            await mediator.Publish(domainEvent, CancellationToken.None);
        dashboard.ClearDomainEvents();
    }

    public async Task UpdateConsumptionThresholdsAsync(
        string homeownerId,
        Dictionary<string, decimal> thresholds)
    {
        var dashboard = await dashboardRepository.FindByOwnerAsync(ClientIdentity.FromHomeowner(homeownerId))
            ?? throw new InvalidOperationException($"No dashboard found for homeowner '{homeownerId}'.");

        dashboard.UpdateConsumptionThresholds(thresholds);
        dashboardRepository.Update(dashboard);
        await unitOfWork.CompleteAsync();
    }
}
