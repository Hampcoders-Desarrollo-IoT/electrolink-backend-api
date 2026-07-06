using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.IAM.Domain.Model.Events;

public record PasswordResetRequestedEvent(
    string UserId,
    string Email,
    string ResetToken,
    DateTime ExpiresAt,
    DateTime OccurredOn
) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
};
