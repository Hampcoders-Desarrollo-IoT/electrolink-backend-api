using Hampcoders.Electrolink.API.Profiles.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Application.Internal.EventHandler;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Events;
using MediatR;

namespace Hampcoders.Electrolink.API.Profiles.Application.Internal.EventHandlers;

public class PaymentFailedEventHandler(
    IProfileRepository profileRepository,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<PaymentFailedEventHandler> logger)
    : IEventHandler<PaymentFailedEvent>
{
    public async Task Handle(PaymentFailedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Profiles] Handling PaymentFailed for User={UserId}, Subscription={SubId}",
            @event.UserId, @event.SubscriptionId);

        var profile = await profileRepository.FindByUserIdAsync(UserId.From(@event.UserId));

        if (profile is null)
        {
            logger.LogWarning("[Profiles] No profile found for UserId={UserId}", @event.UserId);
            return;
        }

        if (profile.Status != Domain.Model.ValueObjects.EProfileStatus.Active)
        {
            logger.LogInformation(
                "[Profiles] Profile {ProfileId} is not Active (status={Status}), skipping suspension",
                profile.ProfileId.Value, profile.Status);
            return;
        }

        profile.Suspend();
        profileRepository.Update(profile);
        await unitOfWork.CompleteAsync();

        foreach (var domainEvent in profile.DomainEvents)
            await mediator.Publish(domainEvent, cancellationToken);
        profile.ClearDomainEvents();

        logger.LogInformation("[Profiles] Profile {ProfileId} suspended due to payment failure",
            profile.ProfileId.Value);
    }
}
