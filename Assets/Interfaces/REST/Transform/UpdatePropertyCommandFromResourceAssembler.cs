using Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Assets.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Transform;

public static class UpdatePropertyCommandFromResourceAssembler
{
    public static UpdatePropertyCommand ToCommandFromResource(UpdatePropertyResource resource, string propertyId)
    {
        var address = Address.Create(
            resource.Address.Street, 
            resource.Address.Number,
            resource.Address.District, 
            resource.Address.City, 
            resource.Address.Country, 
            resource.Address.PostalCode
        );
        
        if (resource is null) throw new ArgumentNullException(nameof(resource));

        return new UpdatePropertyCommand(
            PropertyId.From(propertyId),
            ClientIdentity.FromHomeowner(resource.OwnerId),
            address
        );
    }
}