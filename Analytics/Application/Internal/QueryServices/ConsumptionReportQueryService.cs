using Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Analytics.Domain.Repositories;
using Hampcoders.Electrolink.API.Analytics.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Analytics.Application.Internal.QueryServices;

public class ConsumptionReportQueryService(
    IConsumptionReportRepository reportRepository)
    : IConsumptionReportQueryService
{
    public async Task<List<ConsumptionReport>> GetReportsByHomeownerAsync(string homeownerId)
    {
        return await reportRepository.FindByOwnerAsync(ClientIdentity.FromHomeowner(homeownerId));
    }
}
