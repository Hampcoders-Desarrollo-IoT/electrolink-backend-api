namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Resources;

public record UpdateLocationResource(
    AddressResource? Address,
    GeolocationResource? Geolocation);
