namespace Hampcoders.Electrolink.API.Subscriptions.Interfaces.ACL;

public interface ISubscriptionContextFacade
{
    /// <summary>
    /// Returns whether a homeowner can create a new service request.
    /// </summary>
    Task<bool> CanCreateRequestAsync(string homeownerId);

    /// <summary>
    /// Returns whether a homeowner can mark a request as priority.
    /// </summary>
    Task<bool> CanMarkAsPriorityAsync(string homeownerId);

    /// <summary>
    /// Returns the remaining monthly request quota. Null = unlimited (Premium).
    /// </summary>
    Task<int?> GetRemainingRequestsAsync(string homeownerId);

    /// <summary>
    /// Returns whether a technician has an active Premium subscription.
    /// </summary>
    Task<bool> IsTechnicianPremiumAsync(string technicianId);
    
    /// <summary>
    /// Retorna los datos de elegibilidad del propietario para crear solicitudes.
    /// </summary>
    Task<(bool canCreate, string planType, int? remainingRequests, bool canMarkAsPriority)>
        GetRequestEligibilityAsync(string homeownerId);

    /// <summary>Incrementa el contador de solicitudes del mes para el homeowner.</summary>
    Task<bool> IncrementMonthlyRequestUsageAsync(string homeownerId);

    /// <summary>
    /// Verifica si un técnico tiene suscripción Premium activa.
    /// Usado por Service Design para permitir creación de catálogos.
    /// </summary>
    Task<bool> TechnicianHasPremiumSubscriptionAsync(string technicianId);

    /// <summary>Retorna el tipo de plan del técnico como string. Null si no tiene suscripción.</summary>
    Task<string?> GetTechnicianPlanTypeAsync(string technicianId);

    /// <summary>
    /// Records the service request ID for an IoT installation on the subscription.
    /// </summary>
    Task RecordInstallationServiceRequestAsync(string subscriptionId, string serviceRequestId);
}