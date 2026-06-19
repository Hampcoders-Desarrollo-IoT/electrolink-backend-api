using Hampcoders.Electrolink.API.Analytics.Domain.Services;
using MediatR;

namespace Hampcoders.Electrolink.API.Analytics.Application.Internal.EventHandlers;

public class ConsumptionThresholdsUpdatedEventHandler(
    IConsumptionDashboardCommandService dashboardCommandService,
    ILogger<ConsumptionThresholdsUpdatedEventHandler> logger)
    : INotificationHandler<ConsumptionThresholdsUpdatedIntegrationEvent>
{
    public async Task Handle(ConsumptionThresholdsUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Analytics BC] ConsumptionThresholdsUpdated: homeowner={HomeownerId}, thresholds={Thresholds}",
            notification.OwnerId, notification.Thresholds.Count);

        await dashboardCommandService.UpdateConsumptionThresholdsAsync(
            notification.OwnerId, notification.Thresholds);
    }
}
