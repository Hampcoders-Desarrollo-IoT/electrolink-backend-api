using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using PropertyId = Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects.PropertyId;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Events;

public record TechnicianAssignedEvent(
    RequestId RequestId,
    TechnicianId TechnicianId,
    ClientIdentity Client,
    RecipeId ServiceId,
    DateOnly ScheduledDate,
    DateTime OccurredOn
) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}
