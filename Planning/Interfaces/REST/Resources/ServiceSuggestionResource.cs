namespace Hampcoders.Electrolink.API.Planning.Interfaces.REST.Resources;

public record ServiceSuggestionResource(
    string SuggestionId,
    string PropertyId,
    string AnomalyType,
    string Severity,
    string Status,
    DateTime ExpiresAt,
    DateTime? ViewedAt);
