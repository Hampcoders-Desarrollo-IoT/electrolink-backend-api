using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Monitoring.Domain.Model.Commands;

/// <summary>
/// Command to extend the scheduled time for a service.
/// </summary>
public record ExtendServiceWaitTimeCommand(
    ServiceExecutionId ExecutionId,
    ClientIdentity Owner,
    int ExtendMinutes,
    DateTime RequestedAt
);


