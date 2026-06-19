namespace Hampcoders.Electrolink.API.Processing.Domain.Model.Commands;

public record UpdateCustomThresholdsCommand(
    string  OwnerId,
    float   NominalVoltage,
    float   MaxConsumptionWatts,
    float   MaxCurrentAmps,
    float   MinPowerFactor,
    float   NominalFrequency,
    int     DisconnectionThresholdMin
);
