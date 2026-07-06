using Hampcoders.Electrolink.API.IAM.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.IAM.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.IAM.Interfaces.REST.Transform;

public static class StaffUserResourceFromEntityAssembler
{
    public static StaffUserResource ToResourceFromEntity(User user)
        => new(user.Id.Value, user.Email.Value, user.Role.ToString());
}
