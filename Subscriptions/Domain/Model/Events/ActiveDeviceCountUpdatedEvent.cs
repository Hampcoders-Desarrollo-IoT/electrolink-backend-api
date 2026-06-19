using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Events;

public sealed record ActiveDeviceCountUpdatedEvent(
    string SubscriptionId,
    string UserId,
    int PreviousActiveDeviceCount,
    int NewActiveDeviceCount,
    int PricePerDevice,
    int NewMonthlyAmount,
    DateTime OccurredAt) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = OccurredAt;
}
