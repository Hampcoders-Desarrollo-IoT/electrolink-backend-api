using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Monitoring.Domain.Model.Commands;

/// <summary>
/// Command to create a new service execution from an assignment.
/// </summary>
public record CreateServiceExecutionCommand(
    AssignmentId AssignmentId,
    RequestId RequestId,
    TechnicianId TechnicianId,
    ClientIdentity Owner,
    PropertyId PropertyId,
    RecipeSnapshot RecipeSnapshot,
    DateTime ScheduledDateTime,
    bool IsPriority,
    IoTContextSnapshot? IotContext = null,
    EServiceType ServiceType = EServiceType.Standard,
    bool IsStaffTechnician = false
);
