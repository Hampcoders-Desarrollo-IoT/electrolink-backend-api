using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Processing.Domain.Model.Events;

public record DeviceReconnectedEvent(
    string   DeviceId,
    string   PropertyId,
    string   OwnerId,
    DateTime ReconnectedAt) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    DateTime IEvent.OccurredOn => ReconnectedAt;
}
