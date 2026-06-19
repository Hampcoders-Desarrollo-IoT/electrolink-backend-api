using Hampcoders.Electrolink.API.Planning.Domain.Model.Events;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Aggregates;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;

public class ServiceSuggestion : BaseAggregateRoot
{
    public string SuggestionId { get; private set; } = null!;
    public string ClientId { get; private set; } = null!;
    public string? ProfileId { get; private set; }
    public string PropertyId { get; private set; } = null!;
    public string AnomalyId { get; private set; } = null!;
    public string DeviceId { get; private set; } = null!;
    public string AnomalyType { get; private set; } = null!;
    public string Severity { get; private set; } = null!;
    public ESuggestionStatus Status { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? ViewedAt { get; private set; }
    public DateTime? ConvertedToRequestAt { get; private set; }
    public string? ConvertedToRequestId { get; private set; }

    private ServiceSuggestion() { }

    public static ServiceSuggestion Create(
        string clientId,
        string? profileId,
        string propertyId,
        string anomalyId,
        string deviceId,
        string anomalyType,
        string severity,
        DateTime expiresAt)
    {
        var suggestion = new ServiceSuggestion
        {
            SuggestionId = Guid.NewGuid().ToString("N"),
            ClientId = clientId,
            ProfileId = profileId,
            PropertyId = propertyId,
            AnomalyId = anomalyId,
            DeviceId = deviceId,
            AnomalyType = anomalyType,
            Severity = severity,
            Status = ESuggestionStatus.Pending,
            ExpiresAt = expiresAt,
        };

        suggestion.RaiseDomainEvent(new ServiceSuggestionCreatedEvent(
            suggestion.SuggestionId,
            suggestion.ClientId,
            suggestion.PropertyId,
            suggestion.AnomalyId,
            suggestion.AnomalyType,
            suggestion.Severity,
            suggestion.ExpiresAt,
            DateTime.UtcNow));

        return suggestion;
    }

    public void MarkAsViewed()
    {
        if (Status != ESuggestionStatus.Pending)
            throw new InvalidOperationException($"Suggestion {SuggestionId} cannot be viewed (status: {Status}).");

        Status = ESuggestionStatus.Viewed;
        ViewedAt = DateTime.UtcNow;
    }

    public void Expire()
    {
        if (Status != ESuggestionStatus.Pending)
            return;

        Status = ESuggestionStatus.Expired;
        RaiseDomainEvent(new ServiceSuggestionExpiredEvent(SuggestionId));
    }

    public void ConvertToRequest()
    {
        if (Status != ESuggestionStatus.Pending && Status != ESuggestionStatus.Viewed)
            throw new InvalidOperationException($"Suggestion {SuggestionId} cannot be converted (status: {Status}).");

        Status = ESuggestionStatus.ConvertedToRequest;
        ConvertedToRequestAt = DateTime.UtcNow;
    }

    public void Accept(string requestId)
    {
        if (Status != ESuggestionStatus.Pending && Status != ESuggestionStatus.Viewed)
            throw new InvalidOperationException($"Suggestion {SuggestionId} cannot be accepted (status: {Status}).");

        Status = ESuggestionStatus.ConvertedToRequest;
        ConvertedToRequestAt = DateTime.UtcNow;
        ConvertedToRequestId = requestId;

        RaiseDomainEvent(new ServiceSuggestionAcceptedEvent(
            SuggestionId, ClientId, requestId, DateTime.UtcNow));
    }
}
