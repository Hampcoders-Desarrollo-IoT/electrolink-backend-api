using Hampcoders.Electrolink.API.Planning.Domain.Model.Events;
using Hampcoders.Electrolink.API.Profiles.Interfaces.ACL;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Services;
using MediatR;

namespace Hampcoders.Electrolink.API.Subscriptions.Application.Internal.EventHandlers;

public class ServiceRequestCreatedEventHandler(
    ISubscriptionCommandService commandService,
    IProfilesContextFacade profilesFacade)
    : INotificationHandler<ServiceRequestCreatedEvent>
{
    public async Task Handle(ServiceRequestCreatedEvent notification, CancellationToken cancellationToken)
    {
        var profileId = await profilesFacade.GetProfileIdByHomeownerIdAsync(notification.Client.ClientId);
        if (string.IsNullOrWhiteSpace(profileId))
            return;

        await commandService.Handle(new IncrementMonthlyRequestCounterCommand(
            ProfileId: profileId));
    }
}

