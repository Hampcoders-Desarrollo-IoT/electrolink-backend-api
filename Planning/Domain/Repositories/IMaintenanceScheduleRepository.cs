using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;

namespace Hampcoders.Electrolink.API.Planning.Domain.Repositories;

public interface IMaintenanceScheduleRepository : IBaseRepository<MaintenanceSchedule, ScheduleId>
{
    Task<IEnumerable<MaintenanceSchedule>> FindDueSchedulesAsync(DateTime asOf);
    Task<IEnumerable<MaintenanceSchedule>> FindByCompanyIdAsync(string companyId);
}
