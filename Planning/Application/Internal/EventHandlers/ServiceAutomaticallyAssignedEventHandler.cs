using Hampcoders.Electrolink.API.Planning.Application.Internal.OutboundServices;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Events;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Planning.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Application.Internal.EventHandler;

namespace Hampcoders.Electrolink.API.Planning.Application.Internal.EventHandlers;

/// <summary>
/// Manejador de evento: Cuando una asignación se ejecuta exitosamente.
/// Marca el request como Assigned.
/// Hotspot 4: Reactividad automática a eventos de asignación.
/// </summary>
public class ServiceAutomaticallyAssignedEventHandler(
    IServiceDesignQueryService serviceDesignQueryService,
    IServiceRequestCommandService requestCommandService,
    ExternalMonitoringService serviceOperationFacade,
    ILogger<ServiceAutomaticallyAssignedEventHandler> logger)
    : IEventHandler<ServiceAutomaticallyAssignedEvent>
{
    public async Task Handle(
        ServiceAutomaticallyAssignedEvent @event,
        CancellationToken cancellationToken)
    {
        var request = await serviceDesignQueryService.Handle(new GetServiceRequestByIdQuery(@event.RequestId));
        
        if (request is null)
        {
            logger.LogError("[Planning BC] Original ServiceRequest {RequestId} not found. Cannot trigger SOM execution.", @event.RequestId.Value);
            return;
        }

        try
        {
            await serviceOperationFacade.CreateServiceExecutionAsync(
                @event.AssignmentId.Value,
                @event.RequestId.Value,
                @event.TechnicianId.Value,
                request.Client.ClientId,
                request.PropertyId.Value,
                @event.RecipeSnapshot,
                @event.ScheduledAt,
                @event.IsPriority);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create ServiceExecution for Assignment {AssignmentId}",
                @event.AssignmentId.Value);
        }
    }
}
