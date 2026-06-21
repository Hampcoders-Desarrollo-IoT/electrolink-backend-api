using System.Text.Json;
using System.Text.RegularExpressions;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Infrastructure;
using Hampcoders.Electrolink.API.Shared.Infrastructure.ExternalProviders;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace Hampcoders.Electrolink.API.Planning.Infrastructure.ExternalProviders;

public partial class OpenAIMatchingProvider : IAIMatchingProvider
{
    private readonly ChatClient _chatClient;
    private readonly OpenAISettings _settings;
    private readonly ILogger<OpenAIMatchingProvider> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public OpenAIMatchingProvider(IOptions<OpenAISettings> settings, ILogger<OpenAIMatchingProvider> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        var credential = new ApiKeyCredential(_settings.ApiKey);
        var clientOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri(_settings.BaseUrl)
        };
        var client = new OpenAIClient(credential, clientOptions);
        _chatClient = client.GetChatClient(_settings.ModelId);
    }

    public async Task<IReadOnlyList<ScoredCandidate>> ScoreTechnicianCandidatesAsync(
        MatchingContext context, CancellationToken cancellationToken = default)
    {
        var systemPrompt = await File.ReadAllTextAsync(_settings.SystemPromptPath, cancellationToken);
        var userPrompt = JsonSerializer.Serialize(context, JsonOptions);

        ChatMessage[] messages =
        [
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        ];

        var options = new ChatCompletionOptions
        {
            MaxOutputTokenCount = _settings.MaxTokens
        };

        for (var attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(_settings.TimeoutSeconds));

                var response = await _chatClient.CompleteChatAsync(messages, options, cts.Token);
                var raw = response.Value.Content[0].Text;
                _logger.LogWarning("AI provider raw response: {Response}", raw);
                var cleaned = StripThinkBlock(raw);
                var json = ExtractJson(cleaned);

                try
                {
                    var candidates = JsonSerializer.Deserialize<List<ScoredCandidate>>(json, JsonOptions);
                    if (candidates is not null)
                        return candidates.AsReadOnly();
                }
                catch (JsonException)
                {
                    _logger.LogWarning("Failed to parse AI response as JSON, attempting fallback extraction");
                    var fallbackJson = FallbackExtractJson(cleaned);
                    if (fallbackJson is not null)
                    {
                        var candidates = JsonSerializer.Deserialize<List<ScoredCandidate>>(fallbackJson, JsonOptions);
                        if (candidates is not null)
                            return candidates.AsReadOnly();
                    }
                    _logger.LogWarning("AI returned non-JSON response, returning empty");
                    return [];
                }
            }
            catch (OperationCanceledException) when (attempt < 3)
            {
                _logger.LogWarning("OpenAI matching timed out on attempt {Attempt}, retrying...", attempt);
            }
            catch (Exception ex) when (attempt < 3)
            {
                _logger.LogWarning(ex, "OpenAI matching failed on attempt {Attempt}, retrying...", attempt);
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)), cancellationToken);
            }
            catch (Exception ex)
            {
                throw new AIProviderException("OpenAI", "Failed to score technician candidates after 3 attempts", ex);
            }
        }

        return [];
    }

    private static string StripThinkBlock(string text)
    {
        return ThinkBlockRegex().Replace(text, "");
    }

    private static string ExtractJson(string text)
    {
        var match = JsonCodeBlockRegex().Match(text);
        if (match.Success)
            return match.Groups[1].Value;

        match = AnyCodeBlockRegex().Match(text);
        if (match.Success)
            return match.Groups[1].Value;

        var lastJson = ExtractLastJsonValue(text);
        if (lastJson is not null)
            return lastJson;

        return text;
    }

    private static string? ExtractLastJsonValue(string text)
    {
        var lastArrayStart = text.LastIndexOf('[');
        var lastObjectStart = text.LastIndexOf('{');
        var start = Math.Max(lastArrayStart, lastObjectStart);
        if (start < 0) return null;

        var open = text[start];
        var close = open == '[' ? ']' : '}';
        var depth = 0;
        var inString = false;
        for (var i = start; i < text.Length; i++)
        {
            var c = text[i];
            if (c == '"' && (i == 0 || text[i - 1] != '\\'))
                inString = !inString;
            if (inString) continue;
            if (c == open) depth++;
            else if (c == close)
            {
                depth--;
                if (depth == 0)
                    return text[start..(i + 1)];
            }
        }
        return null;
    }

    private static string? FallbackExtractJson(string text)
    {
        var arrayMatch = JsonArrayRegex().Match(text);
        if (arrayMatch.Success)
            return arrayMatch.Value;
        var objMatch = JsonObjectRegex().Match(text);
        return objMatch.Success ? objMatch.Value : null;
    }

    [GeneratedRegex(@"<think>[\s\S]*?</think>")]
    private static partial Regex ThinkBlockRegex();

    [GeneratedRegex(@"```json\s*([\s\S]*?)\s*```", RegexOptions.Multiline)]
    private static partial Regex JsonCodeBlockRegex();

    [GeneratedRegex(@"```\w*\s*([\s\S]*?)\s*```", RegexOptions.Multiline)]
    private static partial Regex AnyCodeBlockRegex();

    [GeneratedRegex(@"\[[\s\S]*\]")]
    private static partial Regex JsonArrayRegex();

    [GeneratedRegex(@"\{[\s\S]*\}")]
    private static partial Regex JsonObjectRegex();
}
