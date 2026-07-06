using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hampcoders.Electrolink.API.Planning.Infrastructure.Persistence.EFC.Repositories;

public class MaintenanceScheduleRepository(AppDbContext context)
    : BaseRepository<MaintenanceSchedule, ScheduleId>(context), IMaintenanceScheduleRepository
{
    public async Task<IEnumerable<MaintenanceSchedule>> FindDueSchedulesAsync(DateTime asOf)
        => await Context.Set<MaintenanceSchedule>()
            .Where(s => s.IsActive && s.NextVisitDate <= asOf)
            .ToListAsync();

    public async Task<IEnumerable<MaintenanceSchedule>> FindByCompanyIdAsync(string companyId)
        => await Context.Set<MaintenanceSchedule>()
            .Where(s => s.CompanyId == CompanyId.From(companyId))
            .ToListAsync();
}
