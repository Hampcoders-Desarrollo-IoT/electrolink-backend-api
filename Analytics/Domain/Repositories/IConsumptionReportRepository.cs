using Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Analytics.Domain.Repositories;

public interface IConsumptionReportRepository : IBaseRepository<ConsumptionReport, ConsumptionReportId>
{
    Task<List<ConsumptionReport>> FindByOwnerAsync(ClientIdentity owner);
}
