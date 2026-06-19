using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Monitoring.Domain.Model.Events;

public record CircuitToggledRemotelyEvent(
    ServiceExecutionId ExecutionId,
    DeviceId DeviceId,
    PropertyId PropertyId,
    ERelayState TargetState,
    string TechnicianId,
    DateTime ToggledAt) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn => ToggledAt;
}
