namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Queries;

public record GetAvailableMakersInZoneQuery(double Latitude, double Longitude, bool RequireIoTCertification = false);
