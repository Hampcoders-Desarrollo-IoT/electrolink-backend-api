using Hampcoders.Electrolink.API.Analytics.Application.Internal.EventHandlers;
using Hampcoders.Electrolink.API.Processing.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Processing.Domain.Services;
using MediatR;

namespace Hampcoders.Electrolink.API.Processing.Application.Internal.EventHandlers;

public class ConsumptionThresholdsUpdatedEventHandler(
    IDeviceReadingStreamCommandService commandService,
    ILogger<ConsumptionThresholdsUpdatedEventHandler> logger)
    : INotificationHandler<ConsumptionThresholdsUpdatedIntegrationEvent>
{
    public async Task Handle(ConsumptionThresholdsUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("[IoT] ThresholdsUpdated event received for OwnerId: {OwnerId}, thresholdCount: {Count}",
            notification.OwnerId, notification.Thresholds.Count);

        var high = notification.Thresholds.TryGetValue("High", out var h) ? h : 0m;

        await commandService.Handle(new UpdateCustomThresholdsCommand(
            notification.OwnerId,
            220f,
            (float)high,
            20f,
            0.85f,
            60f,
            10));
    }
}
