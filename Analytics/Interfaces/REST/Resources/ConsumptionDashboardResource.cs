namespace Hampcoders.Electrolink.API.Analytics.Interfaces.REST.Resources;

public record ConsumptionDashboardResource(
    string DashboardId,
    string OwnerId,
    string PropertyId,
    List<string> DeviceIds,
    string PlanTier,
    decimal TotalConsumptionKWh,
    decimal CostProjectionAmount,
    string CostProjectionCurrency,
    DateTime LastUpdatedAt,
    List<TimeSeriesResource> TimeSeries,
    List<CircuitSummaryResource> CircuitSummaries);
