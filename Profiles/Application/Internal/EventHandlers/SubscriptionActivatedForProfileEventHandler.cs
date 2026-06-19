using Hampcoders.Electrolink.API.Profiles.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Application.Internal.EventHandler;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Profiles.Application.Internal.EventHandlers;

public class SubscriptionActivatedForProfileEventHandler(
    IProfileRepository profileRepository,
    IUnitOfWork unitOfWork,
    ILogger<SubscriptionActivatedForProfileEventHandler> logger)
    : IEventHandler<SubscriptionActivatedEvent>
{
    public async Task Handle(SubscriptionActivatedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Profiles] Updating subscription tier for User={UserId} to {Plan}",
            @event.UserId, @event.PlanType);

        var profile = await profileRepository.FindByUserIdAsync(UserId.From(@event.UserId));
        if (profile is null)
        {
            logger.LogWarning("[Profiles] No profile found for UserId={UserId}", @event.UserId);
            return;
        }

        profile.UpdateSubscriptionTier(@event.PlanType);
        profileRepository.Update(profile);
        await unitOfWork.CompleteAsync();
    }
}
