using Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Assets.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Assets.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Transform;

public static class CreatePropertyCommandFromResourceAssembler
{
    public static CreatePropertyCommand ToCommandFromResource(CreatePropertyResource resource, string ownerId)
    {
        var address = Address.Create(
            resource.Address.Street, 
            resource.Address.Number,
            resource.Address.District, 
            resource.Address.City, 
            resource.Address.Country, 
            resource.Address.PostalCode
        );
        
        var geolocation = Geolocation.Create(resource.Geolocation.Latitude, resource.Geolocation.Longitude, resource.Geolocation.Accuracy, "MANUAL");

        var propertyType = Enum.TryParse<EPropertyType>(resource.PropertyType, ignoreCase: true, out var parsed)
            ? parsed
            : EPropertyType.Residential;

        return new CreatePropertyCommand(ClientIdentity.FromOwnerId(ownerId), address, geolocation, propertyType);
    }
}
