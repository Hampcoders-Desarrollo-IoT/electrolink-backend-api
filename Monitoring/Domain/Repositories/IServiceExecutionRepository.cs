using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;

namespace Hampcoders.Electrolink.API.Monitoring.Domain.Repositories;

/// <summary>
/// Repository interface for ServiceExecution aggregate root.
/// Defines contracts for persistence operations.
/// </summary>
public interface IServiceExecutionRepository : IBaseRepository<ServiceExecution, ServiceExecutionId>
{
    /// <summary>
    /// Retrieves all service executions for a specific technician.
    /// </summary>
    Task<IEnumerable<ServiceExecution>> FindByTechnicianIdAsync(TechnicianId technicianId);

    /// <summary>
    /// Retrieves all service executions for a specific owner.
    /// </summary>
    Task<IEnumerable<ServiceExecution>> FindByOwnerAsync(ClientIdentity owner);

    /// <summary>
    /// Retrieves a service execution by its assignment ID.
    /// </summary>
    Task<ServiceExecution?> FindByAssignmentIdAsync(AssignmentId assignmentId);

    /// <summary>
    /// Retrieves all active (in-progress or scheduled) service executions.
    /// </summary>
    Task<IEnumerable<ServiceExecution>> FindActiveServiceExecutionAsync();

    /// <summary>
    /// Checks if a service execution exists for a given assignment.
    /// </summary>
    Task<bool> ExistsByAssignmentIdAsync(AssignmentId assignmentId);

    /// <summary>
    /// Retrieves the active service execution for a specific owner.
    /// </summary>
    Task<ServiceExecution?> FindActiveByOwnerAsync(ClientIdentity owner);
}

