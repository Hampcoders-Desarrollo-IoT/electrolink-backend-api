using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hampcoders.Electrolink.API.Planning.Infrastructure.Persistence.EFC.Repositories;

public class ServiceRequestRepository(AppDbContext context) : BaseRepository<ServiceRequest, RequestId>(context), IServiceRequestRepository
{

    public async Task<IEnumerable<ServiceRequest>> FindByClientAsync(ClientIdentity client)
    {
        return await Context.Set<ServiceRequest>()
            .Where(r => r.Client == client)
            .OrderByDescending(r => r.RequestId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceRequest>> FindPendingAssignmentAsync(int? page = null, int? pageSize = null)
    {
        var query = Context.Set<ServiceRequest>()
            .Where(r => r.Status == ERequestStatus.PendingAssignment)
            .OrderByDescending(r => r.IsPriority)
            .ThenBy(r => r.CreatedDate);

        if (page.HasValue && pageSize.HasValue)
        {
            var skip = (page.Value - 1) * pageSize.Value;
            query = (IOrderedQueryable<ServiceRequest>)query.Skip(skip).Take(pageSize.Value);
        }

        return await query.ToListAsync();
    }
}

