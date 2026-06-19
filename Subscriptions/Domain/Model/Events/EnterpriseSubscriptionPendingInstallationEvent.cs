using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Events;

public sealed record EnterpriseSubscriptionPendingInstallationEvent(
    string SubscriptionId,
    string UserId,
    string StripeSubscriptionId,
    int InitialDeviceCount,
    int PricePerDevice,
    DateTime InstallationDeadlineAt,
    DateTime OccurredAt) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = OccurredAt;
}
