using Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Assets.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Transform;

public static class SetPropertyMainPhotoCommandFromResourceAssembler
{
    public static SetPropertyMainPhotoCommand ToCommand(
        string ownerId,
        string propertyId,
        SetPropertyMainPhotoResource resource)
    {
        return new SetPropertyMainPhotoCommand(
            ClientIdentity.FromOwnerId(ownerId),
            PropertyId.From(propertyId),
            resource.ProviderId);
    }
}
