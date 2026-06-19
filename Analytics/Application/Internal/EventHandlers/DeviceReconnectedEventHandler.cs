using Hampcoders.Electrolink.API.Analytics.Domain.Services;
using MediatR;

namespace Hampcoders.Electrolink.API.Analytics.Application.Internal.EventHandlers;

public class DeviceReconnectedEventHandler(
    IAlertLogCommandService alertLogCommandService,
    ILogger<DeviceReconnectedEventHandler> logger)
    : INotificationHandler<DeviceReconnectedIntegrationEvent>
{
    public async Task Handle(DeviceReconnectedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Analytics BC] DeviceReconnected: homeowner={HomeownerId}, device={DeviceId}",
            notification.OwnerId, notification.DeviceId);

        await alertLogCommandService.ResolveAlertAsync(
            notification.OwnerId,
            notification.DeviceId);
    }
}
