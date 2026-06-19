namespace Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;

public record DeviceReading(
    DateTime Timestamp,
    decimal Voltage,
    decimal Current,
    decimal PowerFactor,
    decimal Frequency);

public record ActiveAnomaly(
    string AnomalyId,
    string AnomalyType,
    string Severity,
    DateTime DetectedAt);

public record IoTContextSnapshot(
    string DeviceId,
    IReadOnlyList<DeviceReading> LatestReadings,
    IReadOnlyList<ActiveAnomaly> ActiveAnomalies,
    DateTime CapturedAt);
