using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Application.Internal.EventHandler;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Events;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Events;
using MediatR;

namespace Hampcoders.Electrolink.API.Planning.Application.Internal.EventHandlers;

public class ServiceAutomaticallyAssignedForIoTEventHandler(
    IServiceRequestRepository requestRepository,
    IMediator mediator,
    ILogger<ServiceAutomaticallyAssignedForIoTEventHandler> logger)
    : IEventHandler<ServiceAutomaticallyAssignedEvent>
{
    public async Task Handle(ServiceAutomaticallyAssignedEvent @event, CancellationToken cancellationToken)
    {
        var request = await requestRepository.FindByIdAsync(@event.RequestId);
        if (request is null)
        {
            logger.LogWarning("[Planning] ServiceRequest {RequestId} not found", @event.RequestId.Value);
            return;
        }

        if (string.IsNullOrWhiteSpace(request.SubscriptionId))
        {
            logger.LogInformation("[Planning] Request {RequestId} is not an IoT installation — skipping", @event.RequestId.Value);
            return;
        }

        if (request.PropertyId is null)
        {
            logger.LogWarning("[Planning] IoT installation request {RequestId} has no PropertyId selected — cannot publish scheduled event", @event.RequestId.Value);
            return;
        }

        var externalEvent = new IoTInstallationServiceScheduledExternalEvent(
            PropertyId: request.PropertyId.Value,
            InstallationServiceRequestId: request.RequestId.Value,
            SelectedDeviceId: string.Empty,
            ScheduledAt: @event.ScheduledAt);

        await mediator.Publish(externalEvent, cancellationToken);

        logger.LogInformation(
            "[Planning] IoTInstallationServiceScheduled published for Request={RequestId}, Property={PropertyId}",
            request.RequestId.Value, request.PropertyId.Value);
    }
}
