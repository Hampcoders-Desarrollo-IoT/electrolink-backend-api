using System.ComponentModel.DataAnnotations;

namespace Hampcoders.Electrolink.API.Profiles.Infrastructure.ExternalProviders;

public class ProfilesAISettings
{
    public const string SectionName = "ProfilesAI";

    [Required(ErrorMessage = "AI ApiKey is required")]
    public string ApiKey { get; set; } = string.Empty;

    [Required(ErrorMessage = "AI ModelId is required")]
    public string ModelId { get; set; } = "mixtral-8x7b-32768";

    [Required(ErrorMessage = "AI SystemPromptPath is required")]
    public string SystemPromptPath { get; set; } = string.Empty;

    [Range(1, 16384, ErrorMessage = "MaxTokens must be between 1 and 16384")]
    public int MaxTokens { get; set; } = 4096;

    [Range(1, 300, ErrorMessage = "TimeoutSeconds must be between 1 and 300")]
    public int TimeoutSeconds { get; set; } = 30;
}
