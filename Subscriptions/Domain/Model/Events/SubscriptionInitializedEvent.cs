using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Events;

/// <summary>
/// Domain event: Subscription initialized for new user.
/// Raised when a user profile is completed and basic subscription is created.
/// 
/// Consumed by: Notifications BC (welcome email), Monitoring BC (audit trail)
/// </summary>
public record SubscriptionInitializedEvent(
    string SubscriptionId,
    string UserId,
    string ProfileId,
    string BusinessRole,
    string PlanType,
    string StripeCustomerId,
    DateTime InitializedAt) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = InitializedAt;
}
