using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Processing.Domain.Model.Events;

public record AnomalyDetectedEvent(
    string   AnomalyId,
    string   DeviceId,
    string   PropertyId,
    string   OwnerId,
    string   AnomalyType,
    string   Severity,
    string   DetectionLayer,
    bool     AutoRelayActivated,
    DateTime DetectedAt) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    DateTime IEvent.OccurredOn => DetectedAt;
}
