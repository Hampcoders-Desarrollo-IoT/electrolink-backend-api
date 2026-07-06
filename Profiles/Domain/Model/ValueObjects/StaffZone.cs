namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;

public record StaffZone
{
    public string Region { get; }
    public double CenterLatitude { get; }
    public double CenterLongitude { get; }
    public double RadiusKm { get; }

    private StaffZone(string region, double centerLatitude, double centerLongitude, double radiusKm)
    {
        Region = region;
        CenterLatitude = centerLatitude;
        CenterLongitude = centerLongitude;
        RadiusKm = radiusKm;
    }

    public static StaffZone Create(string region, double centerLatitude, double centerLongitude, double radiusKm)
    {
        if (string.IsNullOrWhiteSpace(region))
            throw new ArgumentException("Region cannot be empty.");
        if (radiusKm <= 0)
            throw new ArgumentException("Radius must be positive.");

        return new StaffZone(region, centerLatitude, centerLongitude, radiusKm);
    }
}
