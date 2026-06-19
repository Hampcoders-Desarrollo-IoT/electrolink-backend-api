namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Resources;

public record CreatePropertyResource(
    AddressResource Address,
    GeolocationResource Geolocation,
    string PropertyType);
