using Hampcoders.Electrolink.API.Assets.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Application.Internal.EventHandler;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Repository;
using MediatR;

namespace Hampcoders.Electrolink.API.Subscriptions.Application.Internal.EventHandlers;

public class DeviceInstalledEventHandler(
    ISubscriptionRepository subscriptionRepository,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<DeviceInstalledEventHandler> logger)
    : IEventHandler<DeviceInstalledEvent>
{
    public async Task Handle(DeviceInstalledEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Subscriptions] DeviceInstalled: Device={DeviceId}, ServiceRequest={InstallationRequestId}",
            @event.DeviceId, @event.InstallationRequestId);

        var subscription = await subscriptionRepository
            .FindByPendingInstallationServiceRequestIdAsync(@event.InstallationRequestId);

        if (subscription is null)
        {
            logger.LogWarning(
                "[Subscriptions] No pending installation subscription found for ServiceRequest={ReqId}",
                @event.InstallationRequestId);
            return;
        }

        subscription.ActivateEnterpriseFullyActive(1, @event.InstallationRequestId);
        subscriptionRepository.Update(subscription);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in subscription.DomainEvents)
            await mediator.Publish(domainEvent, cancellationToken);
        subscription.ClearDomainEvents();

        logger.LogInformation(
            "[Subscriptions] Subscription {SubId} fully activated after device installation",
            subscription.SubscriptionId.Value);
    }
}
