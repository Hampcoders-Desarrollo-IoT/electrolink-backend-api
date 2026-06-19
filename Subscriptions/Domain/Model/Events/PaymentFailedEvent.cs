using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Events;

public sealed record PaymentFailedEvent(
    string SubscriptionId,
    string UserId,
    string ProfileId,
    string StripeInvoiceId,
    DateTime FailedAt) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = FailedAt;
}
