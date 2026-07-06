using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Events;

public record StaffMemberCreatedEvent(
    string StaffMemberId,
    string UserId,
    string FirstName,
    string LastName,
    DateTime OccurredOn
) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}
