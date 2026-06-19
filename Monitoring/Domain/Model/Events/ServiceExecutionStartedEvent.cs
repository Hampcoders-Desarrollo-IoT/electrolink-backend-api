using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Monitoring.Domain.Model.Events;

/// <summary>
/// Event published when a service execution is started.
/// </summary>
public record ServiceExecutionStartedEvent(
    ServiceExecutionId ExecutionId,
    AssignmentId AssignmentId,
    TechnicianId TechnicianId,
    ClientIdentity Owner,
    EExecutionStatus PreviousStatus,
    EExecutionStatus NewStatus,
    DateTime StartedAt,
    DateTime OccurredOn) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}

