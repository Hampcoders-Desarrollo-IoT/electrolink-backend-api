using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Processing.Domain.Model.Events;

public record ReadingIngestedEvent(
    string   StreamId,
    string   DeviceId,
    string   PropertyId,
    string   OwnerId,
    string   ReadingId,
    DateTime ReadingTimestamp,
    string   Source,
    DateTime OccurredAt) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    DateTime IEvent.OccurredOn => OccurredAt;
}
