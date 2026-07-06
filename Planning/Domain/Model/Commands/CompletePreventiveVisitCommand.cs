namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;

public record CompletePreventiveVisitCommand(string ScheduleId, DateTime CompletedAt);
