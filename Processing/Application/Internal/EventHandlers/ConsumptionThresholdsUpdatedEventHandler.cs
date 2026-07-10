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
        var t = notification.Thresholds;
        logger.LogInformation("[IoT] ThresholdsUpdated event received for OwnerId: {OwnerId}, thresholdCount: {Count}",
            notification.OwnerId, t.Count);

        await commandService.Handle(new UpdateCustomThresholdsCommand(
            notification.OwnerId,
            t.TryGetValue("nominalVoltage", out var nv) ? (float)nv : 220f,
            t.TryGetValue("maxConsumptionWatts", out var mcw) ? (float)mcw : 5000f,
            t.TryGetValue("maxCurrentAmps", out var mca) ? (float)mca : 20f,
            t.TryGetValue("minPowerFactor", out var mpf) ? (float)mpf : 0.85f,
            t.TryGetValue("nominalFrequency", out var nf) ? (float)nf : 60f,
            t.TryGetValue("disconnectionThresholdMin", out var dtm) ? (int)dtm : 10));
    }
}
