using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using PropertyId = Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects.PropertyId;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Events;

public record RequestCreatedEvent(
    RequestId RequestId,
    ClientIdentity Client,
    PropertyId PropertyId,
    RecipeId SelectedRecipeId,
    TechnicianId SelectedTechnicianId,
    bool IsPriority,
    List<DateTime> PreferredDates,
    ETimePreference TimePreference,
    DateTime OccurredOn
) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}
