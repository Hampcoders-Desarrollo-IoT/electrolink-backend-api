namespace Hampcoders.Electrolink.API.Monitoring.Interfaces.REST.Resources;

public record ServiceExecutionResource(
    string ExecutionId,
    string AssignmentId,
    string TechnicianId,
    string OwnerId,
    string PropertyId,
    string Status,
    DateTime  ScheduledAt,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    string ServiceName,
    string ServiceCategory,
    decimal TotalPrice,
    int EstimatedDuration,
    bool IsPriority,
    WorkLogSummaryResource WorkLog
);
