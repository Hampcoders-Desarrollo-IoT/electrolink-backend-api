using System.ComponentModel.DataAnnotations;

namespace Hampcoders.Electrolink.API.Shared.Infrastructure.ExternalProviders;

public class GeminiSettings
{
    public const string SectionName = "Gemini";

    [Required(ErrorMessage = "Gemini ApiKey is required")]
    public string ApiKey { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gemini ModelId is required")]
    public string ModelId { get; set; } = "gemini-2.0-flash";

    [Required(ErrorMessage = "Gemini SystemPromptPath is required")]
    public string SystemPromptPath { get; set; } = string.Empty;

    [Range(1, 16384, ErrorMessage = "MaxTokens must be between 1 and 16384")]
    public int MaxTokens { get; set; } = 4096;

    [Range(1, 300, ErrorMessage = "TimeoutSeconds must be between 1 and 300")]
    public int TimeoutSeconds { get; set; } = 120;
}
