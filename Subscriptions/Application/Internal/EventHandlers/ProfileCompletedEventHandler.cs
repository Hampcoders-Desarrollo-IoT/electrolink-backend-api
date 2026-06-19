using Hampcoders.Electrolink.API.Profiles.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Services;
using MediatR;
using ProfileId = Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects.ProfileId;

namespace Hampcoders.Electrolink.API.Subscriptions.Application.Internal.EventHandlers;

public class ProfileCompletedEventHandler(ISubscriptionCommandService commandService)
    : INotificationHandler<ProfileCompletedEvent>
{
    public async Task Handle(ProfileCompletedEvent notification, CancellationToken cancellationToken)
    {
        await commandService.Handle(new CreateSubscriptionCommand(
            UserId: UserId.From(notification.UserId),
            ProfileId: ProfileId.From(notification.ProfileId)));
    }
}

