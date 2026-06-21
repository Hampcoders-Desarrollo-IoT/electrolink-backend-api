using Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Assets.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Transform;

public static class RegisterPropertyPhotoCommandFromResourceAssembler
{
    public static RegisterPropertyPhotoCommand ToCommand(
        string ownerId,
        string propertyId,
        RegisterPropertyPhotoResource resource)
    {
        return new RegisterPropertyPhotoCommand(
            ClientIdentity.FromOwnerId(ownerId),
            PropertyId.From(propertyId),
            resource.ProviderId,
            resource.PublicUrl);
    }
}
