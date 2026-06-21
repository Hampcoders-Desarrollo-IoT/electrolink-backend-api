using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Monitoring.Interfaces.ACL;

/// <summary>
/// Facade for accessing monitoring context information, such as service statuses and counts, in a simplified manner. This interface abstracts the underlying complexities of retrieving monitoring data, providing a clean and straightforward API for clients to interact with the monitoring system.
/// </summary>
public interface IMonitoringContextFacade
{
    /// <summary>
    /// Counts the number of active services associated with a specific recipe. This method provides a quick way to determine how many services are currently active for a given recipe, which can be useful for monitoring and reporting purposes.
    /// </summary>
    Task<int> CountActiveServicesForRecipeAsync(string recipeId);

    /// <summary>
    /// Counts the number of services that are currently in progress for a specific recipe. This method helps to identify how many services are currently being executed for a given recipe, which can be important for tracking the progress and performance of the recipe execution.
    /// </summary>
    Task<int> CountInProgressServicesForRecipeAsync(string recipeId);

    /// <summary>
    /// Checks if a specific service is currently active. This method allows clients to quickly determine the status of a service by its identifier, which can be useful for monitoring and decision-making processes.
    /// </summary>
    Task<bool> IsServiceActiveAsync(string serviceId);

    /// <summary>
    /// Retrieves the execution status of a specific service. This method provides detailed information about the current state of a service, which can be essential for monitoring, debugging, and reporting purposes. The returned status may include information such as whether the service is running, completed, failed, or in any other relevant state.
    /// </summary>
    Task<string?> GetServiceExecutionStatusAsync(string serviceId);
    
    /// <summary>
    /// Retrieves the execution history of a specific service. This method provides a comprehensive log of all the events and status changes that have occurred during the lifecycle of a service, which can be invaluable for monitoring, troubleshooting, and analyzing the performance of the service over time.
    /// </summary>
    /// <param name="assignmentId"></param>
    /// <param name="requestId"></param>
    /// <param name="technicianId"></param>
    /// <param name="clientId"></param>
    /// <param name="propertyId"></param>
    /// <param name="recipeSnapshot"></param>
    /// <param name="scheduledAt"></param>
    /// <param name="isPriority"></param>
    /// <returns></returns>
    Task<string> CreateServiceExecutionAsync(
        string assignmentId,
        string requestId,
        string technicianId,
        string clientId,
        string propertyId,
        RecipeSnapshot recipeSnapshot,
        DateTime scheduledAt,
        bool isPriority);

    /// <summary>
    /// Records a circuit toggle result from IoT Monitoring BC (internal, no auth).
    /// </summary>
    Task RecordCircuitToggleAsync(
        string executionId,
        string deviceId,
        string targetState,
        string actionStatus,
        string? failureReason);
}