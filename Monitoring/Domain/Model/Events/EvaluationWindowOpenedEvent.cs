using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Monitoring.Domain.Model.Events;

/// <summary>
/// Event published when the evaluation window is opened for a completed service.
/// </summary>
public record EvaluationWindowOpenedEvent(
    ServiceExecutionId ExecutionId,
    AssignmentId AssignmentId,
    TechnicianId TechnicianId,
    ClientIdentity Owner,
    DateTime ExpiresAt,
    DateTime OpenedAt,
    DateTime OccurredOn) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}
