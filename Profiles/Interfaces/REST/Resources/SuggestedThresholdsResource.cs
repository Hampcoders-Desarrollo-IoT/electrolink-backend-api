namespace Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Resources;

public record SuggestedThresholdsResource(
    float NominalVoltage,
    float MaxConsumptionWatts,
    float MaxCurrentAmps,
    float MinPowerFactor,
    float NominalFrequency,
    int DisconnectionThresholdMin,
    float NormalLimitAmps = 0.20f,
    float AlertLimitAmps = 0.60f
);
