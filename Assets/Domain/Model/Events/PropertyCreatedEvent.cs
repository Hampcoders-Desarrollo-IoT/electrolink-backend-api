using Hampcoders.Electrolink.API.Assets.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using PropertyId = Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects.PropertyId;

namespace Hampcoders.Electrolink.API.Assets.Domain.Model.Events;

public record PropertyCreatedEvent(
    PropertyId PropertyId,
    ClientIdentity Owner,
    Address Address,
    Geolocation Geolocation,
    EPropertyType PropertyType,
    DateTime OccurredOn
) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
};