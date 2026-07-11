using System.Text.Json.Serialization;

namespace Hampcoders.Electrolink.API.Processing.Interfaces.REST.Resources;

public record DeviceThresholdResource(
    [property: JsonPropertyName("deviceId")] string DeviceId,
    [property: JsonPropertyName("thresholds")] DeviceThresholdValues Thresholds
);

public record DeviceThresholdValues(
    [property: JsonPropertyName("normalLimit")]           float NormalLimitAmps,
    [property: JsonPropertyName("alertLimit")]            float AlertLimitAmps,
    [property: JsonPropertyName("nominalVoltage")]        float NominalVoltage,
    [property: JsonPropertyName("maxConsumptionWatts")]   float MaxConsumptionWatts,
    [property: JsonPropertyName("maxCurrentAmps")]        float MaxCurrentAmps,
    [property: JsonPropertyName("minPowerFactor")]        float MinPowerFactor,
    [property: JsonPropertyName("nominalFrequency")]      float NominalFrequency,
    [property: JsonPropertyName("disconnectionThresholdMin")] int DisconnectionThresholdMin
);
