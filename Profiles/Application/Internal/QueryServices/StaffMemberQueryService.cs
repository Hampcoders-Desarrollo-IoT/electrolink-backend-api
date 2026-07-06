using Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Profiles.Domain.Repositories;
using Hampcoders.Electrolink.API.Profiles.Domain.Services;

namespace Hampcoders.Electrolink.API.Profiles.Application.Internal.QueryServices;

public class StaffMemberQueryService(IStaffMemberRepository staffMemberRepository)
    : IStaffMemberQueryService
{
    public async Task<StaffMember?> Handle(GetStaffMemberByIdQuery query)
        => await staffMemberRepository.FindByIdAsync(
            Shared.Domain.Model.ValueObjects.StaffMemberId.From(query.StaffMemberId));

    public async Task<StaffMember?> Handle(GetStaffMemberByUserIdQuery query)
        => await staffMemberRepository.FindByUserIdAsync(query.UserId);

    public async Task<IEnumerable<StaffMember>> Handle(GetAllStaffMembersQuery query)
        => await staffMemberRepository.ListAsync();

    public async Task<IEnumerable<StaffMember>> Handle(GetAvailableMakersInZoneQuery query)
        => await staffMemberRepository.FindIoTCertifiedInZoneAsync(query.Latitude, query.Longitude);
}
