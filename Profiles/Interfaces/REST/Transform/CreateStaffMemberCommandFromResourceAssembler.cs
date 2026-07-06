using Hampcoders.Electrolink.API.Profiles.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Transform;

public static class CreateStaffMemberCommandFromResourceAssembler
{
    public static CreateStaffMemberCommand ToCommandFromResource(CreateStaffMemberResource resource)
        => new(resource.UserId, resource.FirstName, resource.LastName, resource.PhoneNumber);
}
