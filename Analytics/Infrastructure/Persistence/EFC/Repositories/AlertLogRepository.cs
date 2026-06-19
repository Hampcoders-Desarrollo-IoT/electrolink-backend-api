using Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Analytics.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Analytics.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hampcoders.Electrolink.API.Analytics.Infrastructure.Persistence.EFC.Repositories;

public class AlertLogRepository(AppDbContext context)
    : BaseRepository<AlertLog, AlertLogId>(context), IAlertLogRepository
{
    public async Task<AlertLog?> FindByOwnerAsync(ClientIdentity owner)
    {
        return await Context.Set<AlertLog>()
            .Include(l => l.Entries)
            .FirstOrDefaultAsync(l => l.Owner.ClientId == owner.ClientId && l.Owner.ClientType == owner.ClientType);
    }
}
