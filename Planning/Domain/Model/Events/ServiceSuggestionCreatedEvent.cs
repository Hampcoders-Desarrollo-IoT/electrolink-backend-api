using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Events;

public sealed record ServiceSuggestionCreatedEvent(
    string SuggestionId,
    string ClientId,
    string PropertyId,
    string AnomalyId,
    string AnomalyType,
    string Severity,
    DateTime ExpiresAt,
    DateTime OccurredAt) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    DateTime IEvent.OccurredOn => OccurredAt;
}
