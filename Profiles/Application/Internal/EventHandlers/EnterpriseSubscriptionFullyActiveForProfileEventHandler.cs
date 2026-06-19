using Hampcoders.Electrolink.API.Profiles.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Application.Internal.EventHandler;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Events;

namespace Hampcoders.Electrolink.API.Profiles.Application.Internal.EventHandlers;

public class EnterpriseSubscriptionFullyActiveForProfileEventHandler(
    IProfileRepository profileRepository,
    IUnitOfWork unitOfWork,
    ILogger<EnterpriseSubscriptionFullyActiveForProfileEventHandler> logger)
    : IEventHandler<EnterpriseSubscriptionFullyActiveEvent>
{
    public async Task Handle(EnterpriseSubscriptionFullyActiveEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Profiles] Updating subscription tier for User={UserId} to {Plan}",
            @event.UserId, @event.EnterprisePlan);

        var profile = await profileRepository.FindByUserIdAsync(UserId.From(@event.UserId));
        if (profile is null)
        {
            logger.LogWarning("[Profiles] No profile found for UserId={UserId}", @event.UserId);
            return;
        }

        profile.UpdateSubscriptionTier(@event.EnterprisePlan);
        profileRepository.Update(profile);
        await unitOfWork.CompleteAsync();
    }
}
