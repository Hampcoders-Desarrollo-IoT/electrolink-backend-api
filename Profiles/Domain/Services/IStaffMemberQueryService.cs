using Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.Queries;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Services;

public interface IStaffMemberQueryService
{
    Task<StaffMember?> Handle(GetStaffMemberByIdQuery query);
    Task<StaffMember?> Handle(GetStaffMemberByUserIdQuery query);
    Task<IEnumerable<StaffMember>> Handle(GetAllStaffMembersQuery query);
    Task<IEnumerable<StaffMember>> Handle(GetAvailableMakersInZoneQuery query);
}
