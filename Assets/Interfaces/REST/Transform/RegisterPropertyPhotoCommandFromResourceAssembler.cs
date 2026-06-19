using Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Assets.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Transform;

public static class RegisterPropertyPhotoCommandFromResourceAssembler
{
    public static RegisterPropertyPhotoCommand ToCommand(
        string homeownerId,
        string propertyId,
        RegisterPropertyPhotoResource resource)
    {
        return new RegisterPropertyPhotoCommand(
            ClientIdentity.FromHomeowner(homeownerId),
            PropertyId.From(propertyId),
            resource.ProviderId,
            resource.PublicUrl);
    }
}
