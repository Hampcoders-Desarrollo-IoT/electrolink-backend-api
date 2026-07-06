using Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Profiles.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hampcoders.Electrolink.API.Profiles.Infrastructure.Persistence.EFC.Repositories;

public class StaffMemberRepository(AppDbContext context)
    : BaseRepository<StaffMember, StaffMemberId>(context), IStaffMemberRepository
{
    public async Task<StaffMember?> FindByUserIdAsync(string userId)
        => await Context.Set<StaffMember>()
            .FirstOrDefaultAsync(s => s.UserId == UserId.From(userId));

    public async Task<IEnumerable<StaffMember>> FindIoTCertifiedInZoneAsync(double latitude, double longitude)
        => await Context.Set<StaffMember>()
            .Where(s => s.IsActive && s.AssignedZone != null)
            .ToListAsync();

    public async Task<IEnumerable<StaffMember>> FindAllActiveAsync()
        => await Context.Set<StaffMember>()
            .Where(s => s.IsActive)
            .ToListAsync();
}
