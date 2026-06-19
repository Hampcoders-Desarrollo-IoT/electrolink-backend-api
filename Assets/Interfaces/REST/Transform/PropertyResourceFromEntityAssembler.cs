using Hampcoders.Electrolink.API.Assets.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Assets.Interfaces.REST.Resources;
using System.Linq;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Transform;

public static class PropertyResourceFromEntityAssembler
{
    public static PropertyResource ToResourceFromEntity(Property entity)
        => new PropertyResource(
            entity.Id.Value,
            entity.Owner.ClientId,
            new AddressResource(
                entity.Address.Street, entity.Address.Number, entity.Address.District, entity.Address.City,
                entity.Address.Country, entity.Address.PostalCode),
            new GeolocationResource(
                entity.Geolocation.Latitude, entity.Geolocation.Longitude,
                entity.Geolocation.Accuracy, entity.Geolocation.Source),
            entity.Status.ToString(),
            entity.IsActive,
            entity.PropertyType.ToString(),
            entity.MainPhotoProviderId,
            entity.Photos.Select(p => PropertyPhotoResourceFromEntityAssembler.ToResource(p)).ToList()
        );
}