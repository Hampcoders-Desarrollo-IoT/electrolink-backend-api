using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Monitoring.Domain.Model.Events;

/// <summary>
/// Event published when a service wait time is extended.
/// </summary>
public record ServiceWaitTimeExtendedEvent(
    ServiceExecutionId ExecutionId,
    ClientIdentity Owner,
    int ExtendedByMinutes,
    DateTime NewDeadline,
    DateTime ExtendedAt,
    DateTime OccurredOn) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}
