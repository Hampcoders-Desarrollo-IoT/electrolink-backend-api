using System.ComponentModel.DataAnnotations;

namespace Hampcoders.Electrolink.API.Shared.Infrastructure.ExternalProviders;

public class OpenAISettings
{
    public const string SectionName = "OpenAI";

    [Required(ErrorMessage = "OpenAI ApiKey is required")]
    public string ApiKey { get; set; } = string.Empty;

    [Required(ErrorMessage = "OpenAI ModelId is required")]
    public string ModelId { get; set; } = string.Empty;

    public string BaseUrl { get; set; } = "https://api.openai.com/v1";

    [Required(ErrorMessage = "OpenAI SystemPromptPath is required")]
    public string SystemPromptPath { get; set; } = string.Empty;

    [Range(1, 16384, ErrorMessage = "MaxTokens must be between 1 and 16384")]
    public int MaxTokens { get; set; } = 4096;

    [Range(1, 300, ErrorMessage = "TimeoutSeconds must be between 1 and 300")]
    public int TimeoutSeconds { get; set; } = 30;

    [Range(1, 60, ErrorMessage = "MatchingTimeoutSeconds must be between 1 and 60")]
    public int MatchingTimeoutSeconds { get; set; } = 8;

    [Range(1, 50, ErrorMessage = "MaxCandidatesForAI must be between 1 and 50")]
    public int MaxCandidatesForAI { get; set; } = 20;
}
