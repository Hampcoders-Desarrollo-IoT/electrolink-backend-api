using Hampcoders.Electrolink.API.Analytics.Application.Internal.Services;
using Hampcoders.Electrolink.API.Analytics.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Analytics.Interfaces.ACL.Services;

public class AnalyticsContextFacade(
    IConsumptionDashboardRepository dashboardRepository,
    IDashboardProjectionService projectionService,
    IAlertLogRepository alertLogRepository,
    ITechnicianMetricsRepository metricsRepository,
    IConsumptionReportRepository reportRepository)
    : IAnalyticsContextFacade
{
    public async Task<bool> HasActiveAlertsAsync(        string homeownerId)
    {
        var alertLog = await alertLogRepository.FindByOwnerAsync(ClientIdentity.FromOwnerId(homeownerId));
        return alertLog?.Entries.Any(e => e.Status == Domain.Model.Enums.AlertStatus.Active) ?? false;
    }

    public async Task<decimal> GetCurrentPeriodConsumptionAsync(string homeownerId)
    {
        var dashboard = await dashboardRepository.FindByOwnerAsync(ClientIdentity.FromOwnerId(homeownerId));
        if (dashboard == null) return 0;

        var timeSeries = await projectionService.GetTimeSeriesAsync(dashboard.DashboardId.Value);
        return timeSeries
            .Where(ts => ts.Timestamp >= DateTime.UtcNow.AddDays(-30))
            .Sum(ts => ts.KilowattHours);
    }

    public async Task<decimal> GetTechnicianAverageRatingAsync(string technicianId)
    {
        var metrics = await metricsRepository.FindCurrentPeriodByTechnicianAsync(
            TechnicianId.From(technicianId));
        return metrics?.AverageRating ?? 0;
    }

    public async Task<bool> HasReportsAvailableAsync(string homeownerId)
    {
        var reports = await reportRepository.FindByOwnerAsync(ClientIdentity.FromOwnerId(homeownerId));
        return reports.Count > 0;
    }
}
