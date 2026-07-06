using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Events;

public record ServiceAutomaticallyAssignedEvent(
    AssignmentId AssignmentId,
    RequestId RequestId,
    TechnicianId TechnicianId,
    RecipeSnapshot RecipeSnapshot,
    DateTime ScheduledAt,
    bool IsPriority,
    bool RequiresIoTCertifiedTechnician,
    DateTime OccurredOn
) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}
