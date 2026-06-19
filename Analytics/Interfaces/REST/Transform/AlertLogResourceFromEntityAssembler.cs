using Hampcoders.Electrolink.API.Analytics.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Analytics.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.Analytics.Interfaces.REST.Transform;

public static class AlertLogResourceFromEntityAssembler
{
    public static AlertLogResource ToResourceFromEntity(AlertLog log)
    {
        return new AlertLogResource(
            log.LogId.Value,
            log.Owner.ClientId,
            log.Entries.Select(e => new AlertEntryResource(
                e.EntryId.Value,
                e.SourceEventId.Value,
                e.SourceBC.ToString(),
                e.AlertType.ToString(),
                e.Severity.ToString(),
                e.Status.ToString(),
                e.CircuitId,
                e.LinkedServiceRequestId?.Value,
                e.TriggeredAt,
                e.AcknowledgedAt,
                e.ResolvedAt)).ToList());
    }
}
