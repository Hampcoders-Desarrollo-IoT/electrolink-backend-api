using Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Transform;

public static class GetPropertyPhotoUploadUrlCommandFromResourceAssembler
{
    public static GetPropertyPhotoUploadUrlCommand ToCommand(string homeownerId, string propertyId)
    {
        return new GetPropertyPhotoUploadUrlCommand(
            ClientIdentity.FromHomeowner(homeownerId),
            PropertyId.From(propertyId));
    }
}
