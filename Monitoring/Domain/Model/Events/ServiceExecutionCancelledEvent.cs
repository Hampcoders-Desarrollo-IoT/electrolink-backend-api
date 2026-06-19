using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Monitoring.Domain.Model.Events;

/// <summary>
/// Event published when a service execution is cancelled.
/// </summary>
public record ServiceExecutionCancelledEvent(
    ServiceExecutionId ExecutionId,
    AssignmentId AssignmentId,
    RequestId RequestId,
    TechnicianId TechnicianId,
    ClientIdentity Owner,
    ECancelledBy CancelledBy,
    string Reason,
    string? Notes,
    string PreviousStatus,
    bool RequestReassignment,
    DateTime CancelledAt,
    DateTime OccurredOn) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}
