using Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Analytics.Domain.Repositories;
using Hampcoders.Electrolink.API.Analytics.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Analytics.Application.Internal.QueryServices;

public class AlertLogQueryService(
    IAlertLogRepository alertLogRepository)
    : IAlertLogQueryService
{
    public async Task<AlertLog?> GetAlertHistoryAsync(
        string homeownerId,
        string? filterType = null,
        string? filterSeverity = null,
        string? filterStatus = null,
        DateTime? fromDate = null)
    {
        return await alertLogRepository.FindByOwnerAsync(ClientIdentity.FromOwnerId(homeownerId));
    }

    public async Task<AlertLog?> GetAnomalyImpactSummaryAsync(
        string homeownerId,
        DateTime fromDate,
        DateTime toDate)
    {
        return await alertLogRepository.FindByOwnerAsync(ClientIdentity.FromOwnerId(homeownerId));
    }
}
