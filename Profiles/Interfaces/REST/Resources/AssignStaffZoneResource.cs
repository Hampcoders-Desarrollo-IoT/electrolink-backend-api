namespace Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Resources;

public record AssignStaffZoneResource(
    string Region,
    double CenterLatitude,
    double CenterLongitude,
    double RadiusKm
);
