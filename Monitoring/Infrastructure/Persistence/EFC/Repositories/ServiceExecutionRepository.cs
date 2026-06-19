using Microsoft.EntityFrameworkCore;
using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Monitoring.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace Hampcoders.Electrolink.API.Monitoring.Infrastructure.Persistence.EFC.Repositories;

public class ServiceExecutionRepository(AppDbContext context)
    : BaseRepository<ServiceExecution, ServiceExecutionId>(context), IServiceExecutionRepository
{
    public async Task<IEnumerable<ServiceExecution>> FindByTechnicianIdAsync(TechnicianId technicianId)
        => await Context.Set<ServiceExecution>()
            .Where(se => se.TechnicianId == technicianId)
            .Include(se => se.WorkPhotos)
            .Include(se => se.ComponentSubstitutions)
            .ToListAsync();

    public async Task<IEnumerable<ServiceExecution>> FindByOwnerAsync(ClientIdentity owner)
        => await Context.Set<ServiceExecution>()
            .Where(se => se.Owner == owner)
            .Include(se => se.WorkPhotos)
            .Include(se => se.ComponentSubstitutions)
            .ToListAsync();

    public async Task<ServiceExecution?> FindByAssignmentIdAsync(AssignmentId assignmentId)
        => await Context.Set<ServiceExecution>()
            .Include(se => se.WorkPhotos)
            .Include(se => se.ComponentSubstitutions)
            .FirstOrDefaultAsync(se => se.AssignmentId == assignmentId);

    public async Task<IEnumerable<ServiceExecution>> FindActiveServiceExecutionAsync()
        => await Context.Set<ServiceExecution>()
            .Where(se => se.Status == EExecutionStatus.Notified
                      || se.Status == EExecutionStatus.EnRoute
                      || se.Status == EExecutionStatus.Arrived
                      || se.Status == EExecutionStatus.InProgress)
            .Include(se => se.WorkPhotos)
            .Include(se => se.ComponentSubstitutions)
            .ToListAsync();

    public async Task<ServiceExecution?> FindActiveByOwnerAsync(ClientIdentity owner)
        => await Context.Set<ServiceExecution>()
            .Where(se => se.Owner == owner
                      && (se.Status == EExecutionStatus.Notified
                       || se.Status == EExecutionStatus.EnRoute
                       || se.Status == EExecutionStatus.Arrived
                       || se.Status == EExecutionStatus.InProgress))
            .Include(se => se.WorkPhotos)
            .Include(se => se.ComponentSubstitutions)
            .FirstOrDefaultAsync();

    public async Task<bool> ExistsByAssignmentIdAsync(AssignmentId assignmentId)
        => await Context.Set<ServiceExecution>()
            .AnyAsync(se => se.AssignmentId == assignmentId);
}
