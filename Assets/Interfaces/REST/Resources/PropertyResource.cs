using System.Collections.Generic;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Resources;

public record PropertyResource(
    string PropertyId,
    string OwnerId,
    AddressResource Address,
    GeolocationResource Geolocation,
    string Status,
    bool IsActive,
    string PropertyType,
    string? MainPhotoProviderId,
    IEnumerable<PropertyPhotoResource> Photos
);