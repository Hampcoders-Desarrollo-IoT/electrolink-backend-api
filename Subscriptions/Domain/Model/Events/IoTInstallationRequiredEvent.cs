using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Events;

public sealed record IoTInstallationRequiredEvent(
    string SubscriptionId,
    string UserId,
    int RequiredDeviceCount,
    DateTime InstallationDeadlineAt,
    DateTime OccurredAt) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = OccurredAt;
}
