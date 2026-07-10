using System.ComponentModel.DataAnnotations;

namespace Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Resources;

public record AdjustThresholdsResource(
    [Required] string AnalysisId,
    [Required] string UserMessage
);
