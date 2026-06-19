using Hampcoders.Electrolink.API.Analytics.Domain.Services;
using MediatR;

namespace Hampcoders.Electrolink.API.Analytics.Application.Internal.EventHandlers;

public class ReadingIngestedEventHandler(
    IConsumptionDashboardCommandService dashboardCommandService,
    ILogger<ReadingIngestedEventHandler> logger)
    : INotificationHandler<ReadingIngestedIntegrationEvent>
{
    public async Task Handle(ReadingIngestedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Analytics BC] ReadingIngested: homeowner={HomeownerId}, device={DeviceId}, circuit={CircuitId}",
            notification.OwnerId, notification.DeviceId, notification.CircuitId);

        await dashboardCommandService.UpdateConsumptionDashboardAsync(
            notification.OwnerId,
            notification.DeviceId,
            notification.CircuitId,
            notification.KilowattHours,
            notification.Voltage,
            notification.Current,
            notification.ReadingTimestamp);
    }
}
