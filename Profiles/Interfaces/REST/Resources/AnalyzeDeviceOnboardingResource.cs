using System.ComponentModel.DataAnnotations;

namespace Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Resources;

public record AnalyzeDeviceOnboardingResource(
    [Required] string DeviceType,
    [Required] [Range(1, 1000)] float NominalVoltage,
    [Required] [Range(1, 24)] float DailyUsageHours,
    [Required] [Range(1, 100)] int Occupants,
    [Required] string LocationType,
    [Required] string PrimaryUse
);
