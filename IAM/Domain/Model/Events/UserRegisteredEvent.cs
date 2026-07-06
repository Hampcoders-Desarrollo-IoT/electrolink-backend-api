using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.IAM.Domain.Model.Events;

public record UserRegisteredEvent(
    string UserId,
    string Username,
    DateTime OccurredOn,
    string Role,
    string? AccessRole = null
) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
};
