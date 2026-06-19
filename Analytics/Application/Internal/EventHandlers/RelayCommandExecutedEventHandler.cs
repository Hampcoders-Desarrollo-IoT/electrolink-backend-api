using Hampcoders.Electrolink.API.Analytics.Domain.Services;
using MediatR;

namespace Hampcoders.Electrolink.API.Analytics.Application.Internal.EventHandlers;

public class RelayCommandExecutedEventHandler(
    IAlertLogCommandService alertLogCommandService,
    ILogger<RelayCommandExecutedEventHandler> logger)
    : INotificationHandler<RelayCommandExecutedIntegrationEvent>
{
    public async Task Handle(RelayCommandExecutedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Analytics BC] RelayCommandExecuted: homeowner={HomeownerId}, device={DeviceId}, circuit={CircuitId}",
            notification.OwnerId, notification.DeviceId, notification.CircuitId);

        await alertLogCommandService.RecordCircuitToggleAlertAsync(
            notification.OwnerId,
            notification.DeviceId,
            notification.CircuitId);
    }
}
