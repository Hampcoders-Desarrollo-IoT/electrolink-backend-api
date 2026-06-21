using Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Assets.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Transform;

public static class UpdatePropertyLocationCommandFromResourceAssembler
{
    public static UpdatePropertyLocationCommand ToCommandFromResource(UpdateLocationResource resource, string propertyId)
    {
        Address? address = resource.Address is not null
            ? Address.Create(
                resource.Address.Street,
                resource.Address.Number,
                resource.Address.District,
                resource.Address.City,
                resource.Address.Country,
                resource.Address.PostalCode)
            : null;

        Geolocation? geolocation = resource.Geolocation is not null
            ? Geolocation.Create(
                resource.Geolocation.Latitude,
                resource.Geolocation.Longitude,
                resource.Geolocation.Accuracy,
                resource.Geolocation.Source)
            : null;

        return new UpdatePropertyLocationCommand(PropertyId.From(propertyId), address, geolocation);
    }
}
