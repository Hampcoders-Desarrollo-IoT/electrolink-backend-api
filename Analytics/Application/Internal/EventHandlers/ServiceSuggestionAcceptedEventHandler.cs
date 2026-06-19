using Hampcoders.Electrolink.API.Analytics.Domain.Services;
using MediatR;

namespace Hampcoders.Electrolink.API.Analytics.Application.Internal.EventHandlers;

public class ServiceSuggestionAcceptedEventHandler(
    IAlertLogCommandService alertLogCommandService,
    ILogger<ServiceSuggestionAcceptedEventHandler> logger)
    : INotificationHandler<ServiceSuggestionAcceptedIntegrationEvent>
{
    public async Task Handle(ServiceSuggestionAcceptedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Analytics BC] ServiceSuggestionAccepted: homeowner={HomeownerId}, suggestion={SuggestionId}",
            notification.OwnerId, notification.ServiceSuggestionId);

        await alertLogCommandService.LinkAlertToServiceRequestAsync(
            notification.OwnerId,
            notification.ServiceSuggestionId,
            notification.ServiceSuggestionId);
    }
}
