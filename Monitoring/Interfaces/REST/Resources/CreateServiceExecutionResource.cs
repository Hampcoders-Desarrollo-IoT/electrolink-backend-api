namespace Hampcoders.Electrolink.API.Monitoring.Interfaces.REST.Resources;

public record CreateServiceExecutionResource(
    string AssignmentId,     
    string RequestId,
    string TechnicianId,
    string OwnerId,
    string PropertyId,
    RecipeSnapshotResource RecipeSnapshot,
    DateTime ScheduledDateTime,
    bool IsPriority
);