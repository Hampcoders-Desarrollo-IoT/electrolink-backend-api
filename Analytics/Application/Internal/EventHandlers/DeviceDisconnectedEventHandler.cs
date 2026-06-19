using Hampcoders.Electrolink.API.Analytics.Domain.Services;
using MediatR;

namespace Hampcoders.Electrolink.API.Analytics.Application.Internal.EventHandlers;

public class DeviceDisconnectedEventHandler(
    IAlertLogCommandService alertLogCommandService,
    ILogger<DeviceDisconnectedEventHandler> logger)
    : INotificationHandler<DeviceDisconnectedIntegrationEvent>
{
    public async Task Handle(DeviceDisconnectedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Analytics BC] DeviceDisconnected: homeowner={HomeownerId}, device={DeviceId}",
            notification.OwnerId, notification.DeviceId);

        await alertLogCommandService.RecordDeviceDisconnectionAlertAsync(
            notification.OwnerId,
            notification.DeviceId,
            circuitId: null);
    }
}
