using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Events;

public record PreventiveMaintenanceScheduledEvent(
    string ScheduleId,
    string CompanyId,
    string PropertyId,
    string Frequency,
    DateTime NextVisitDate,
    DateTime OccurredOn
) : IEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}
