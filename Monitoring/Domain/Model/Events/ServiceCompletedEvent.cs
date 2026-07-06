using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Monitoring.Domain.Model.Events;

/// <summary>
/// Event published when a service execution is completed.
/// </summary>
public record ServiceCompletedEvent(
    ServiceExecutionId ExecutionId,
    AssignmentId AssignmentId,
    RequestId RequestId,
    TechnicianId TechnicianId,
    ClientIdentity Owner,
    PropertyId PropertyId,
    string ServiceCategory,
    IReadOnlyList<ComponentUsage> ComponentsActuallyUsed,
    bool HasComponentOverage,
    IReadOnlyList<string> PhotosUrls,
    string WorkSummary,
    DateTime CompletedAt,
    DateTime OccurredOn,
    bool IsStaffTechnician = false) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}

