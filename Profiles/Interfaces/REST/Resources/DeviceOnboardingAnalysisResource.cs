namespace Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Resources;

public record DeviceOnboardingAnalysisResource(
    string Reasoning,
    string Narrative,
    SuggestedThresholdsResource SuggestedThresholds,
    string? AnalysisId = null
);
