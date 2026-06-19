using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Events;

public record ServiceRequestCreatedEvent(
    RequestId RequestId,
    ClientIdentity Client,
    PropertyId PropertyId,
    RecipeId RecipeId,
    TechnicianId TechnicianId,
    RecipeSnapshot RecipeSnapshot,
    bool IsPriority,
    DateTime OccurredOn
) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}

