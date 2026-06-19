using Hampcoders.Electrolink.API.Analytics.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Analytics.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.Analytics.Interfaces.REST.Transform;

public static class ConsumptionDashboardResourceFromEntityAssembler
{
    public static ConsumptionDashboardResource ToResourceFromEntity(DashboardView view)
    {
        return new ConsumptionDashboardResource(
            view.Dashboard.DashboardId.Value,
            view.Dashboard.Owner.ClientId,
            view.Dashboard.PropertyId.Value,
            view.Dashboard.DeviceIds.Select(d => d.Value).ToList(),
            view.Dashboard.PlanTier.ToString(),
            view.TimeSeries.Sum(ts => ts.KilowattHours),
            view.Dashboard.CostProjection.Amount,
            view.Dashboard.CostProjection.Currency,
            view.Dashboard.LastUpdatedAt,
            view.TimeSeries.Select(ts => new TimeSeriesResource(
                ts.Timestamp, ts.KilowattHours, ts.Granularity)).ToList(),
            view.CircuitSummaries.Select(c => new CircuitSummaryResource(
                c.CircuitId, c.TotalKilowattHours, c.PeakVoltage,
                c.PeakCurrent, c.LastReadingAt)).ToList());
    }
}
