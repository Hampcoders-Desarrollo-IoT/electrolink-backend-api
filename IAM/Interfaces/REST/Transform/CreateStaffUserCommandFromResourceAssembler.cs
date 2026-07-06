using Hampcoders.Electrolink.API.IAM.Domain.Model.Commands;
using Hampcoders.Electrolink.API.IAM.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.IAM.Interfaces.REST.Transform;

public static class CreateStaffUserCommandFromResourceAssembler
{
    public static CreateStaffUserCommand ToCommandFromResource(CreateStaffUserResource resource)
        => new(
            resource.Email,
            resource.Password,
            resource.PasswordConfirmation,
            resource.AccessRole,
            resource.FirstName,
            resource.LastName,
            resource.PhoneNumber);
}
