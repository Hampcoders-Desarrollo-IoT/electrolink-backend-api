using Hampcoders.Electrolink.API.Analytics.Domain.Services;
using MediatR;

namespace Hampcoders.Electrolink.API.Analytics.Application.Internal.EventHandlers;

public class AnomalyResolvedEventHandler(
    IAlertLogCommandService alertLogCommandService,
    ILogger<AnomalyResolvedEventHandler> logger)
    : INotificationHandler<AnomalyResolvedIntegrationEvent>
{
    public async Task Handle(AnomalyResolvedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Analytics BC] AnomalyResolved: homeowner={HomeownerId}, alert={AlertId}",
            notification.OwnerId, notification.AlertId);

        await alertLogCommandService.ResolveAlertAsync(
            notification.OwnerId,
            notification.AlertId);
    }
}
