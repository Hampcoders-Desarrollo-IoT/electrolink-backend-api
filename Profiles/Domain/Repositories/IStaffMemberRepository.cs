using Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Repositories;

public interface IStaffMemberRepository : IBaseRepository<StaffMember, StaffMemberId>
{
    Task<StaffMember?> FindByUserIdAsync(string userId);
    Task<IEnumerable<StaffMember>> FindIoTCertifiedInZoneAsync(double latitude, double longitude);
    Task<IEnumerable<StaffMember>> FindAllActiveAsync();
}
