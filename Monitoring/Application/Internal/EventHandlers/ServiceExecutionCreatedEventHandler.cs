using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using MediatR;

namespace Hampcoders.Electrolink.API.Monitoring.Application.Internal.EventHandlers;

public class ServiceExecutionCreatedEventHandler(ILogger<ServiceExecutionCreatedEventHandler> logger)
    : INotificationHandler<ServiceExecutionCreatedEvent>
{
    public async Task Handle(ServiceExecutionCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Monitoring BC] ServiceExecution {ExecutionId} created for homeowner {HomeownerId}.",
            notification.ExecutionId.Value, notification.Owner.ClientId);
        await Task.CompletedTask;
    }
}
