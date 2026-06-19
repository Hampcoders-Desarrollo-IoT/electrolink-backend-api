using Hampcoders.Electrolink.API.Planning.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Application.Internal.EventHandler;
using Microsoft.Extensions.Logging;

namespace Hampcoders.Electrolink.API.Planning.Application.Internal.EventHandlers;

/// <summary>
/// Handles RequestCreatedEvent for internal processing like logging and analytics.
/// </summary>
public class RequestCreatedEventHandler(ILogger<RequestCreatedEventHandler> logger) 
    : IEventHandler<RequestCreatedEvent>
{
    public Task Handle(RequestCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Request created: {RequestId} for Client: {Client} at {Timestamp}",
            notification.RequestId,
            notification.Client,
            notification.OccurredOn);

        return Task.CompletedTask;
    }
}

