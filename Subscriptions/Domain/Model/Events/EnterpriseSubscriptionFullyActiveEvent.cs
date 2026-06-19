using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Events;

public sealed record EnterpriseSubscriptionFullyActiveEvent(
    string SubscriptionId,
    string UserId,
    string EnterprisePlan,
    int ActiveDeviceCount,
    int PricePerDevice,
    DateTime OccurredAt) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = OccurredAt;
}
