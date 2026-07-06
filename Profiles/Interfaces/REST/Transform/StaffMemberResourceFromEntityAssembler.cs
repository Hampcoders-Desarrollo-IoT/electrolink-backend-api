using Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Transform;

public static class StaffMemberResourceFromEntityAssembler
{
    public static StaffMemberResource ToResourceFromEntity(StaffMember member)
        => new(
            member.Id.Value,
            member.UserId.Value,
            member.FirstName,
            member.LastName,
            member.PhoneNumber,
            member.IsIoTCertified,
            member.AssignedZone?.Region,
            member.IsActive,
            member.HireDate);
}
