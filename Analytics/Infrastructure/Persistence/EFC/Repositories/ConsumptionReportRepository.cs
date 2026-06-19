using Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Analytics.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hampcoders.Electrolink.API.Analytics.Infrastructure.Persistence.EFC.Repositories;

public class ConsumptionReportRepository(AppDbContext context)
    : BaseRepository<ConsumptionReport, ConsumptionReportId>(context), IConsumptionReportRepository
{
    public async Task<List<ConsumptionReport>> FindByOwnerAsync(ClientIdentity owner)
    {
        return await Context.Set<ConsumptionReport>()
            .Where(r => r.RequestedBy.ClientId == owner.ClientId && r.RequestedBy.ClientType == owner.ClientType)
            .ToListAsync();
    }
}
