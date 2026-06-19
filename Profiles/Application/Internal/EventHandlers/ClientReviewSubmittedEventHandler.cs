using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Events;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Profiles.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Application.Internal.EventHandler;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Profiles.Application.Internal.EventHandlers;

public class ClientReviewSubmittedEventHandler(
    IProfileRepository profileRepository,
    IUnitOfWork unitOfWork,
    ILogger<ClientReviewSubmittedEventHandler> logger)
    : IEventHandler<ClientReviewSubmittedEvent>
{
    public async Task Handle(ClientReviewSubmittedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Profiles] Processing client review: ReviewedId={ReviewedId}, Rating={Rating}",
            @event.ReviewedId, @event.Rating);

        Profile? profile = null;
        if (@event.ReviewedId.StartsWith("ho-"))
            profile = await profileRepository.FindByHomeownerIdAsync(
                HomeownerId.From(@event.ReviewedId));
        else if (@event.ReviewedId.StartsWith("comp-"))
            profile = await profileRepository.FindByCompanyIdAsync(
                CompanyId.From(@event.ReviewedId));

        if (profile?.Homeowner is not null)
        {
            profile.Homeowner.UpdateRating(@event.Rating);
            profileRepository.Update(profile);
            await unitOfWork.CompleteAsync();
            logger.LogInformation(
                "[Profiles] HomeOwner {HomeownerId} rating updated to {AverageRating}",
                @event.ReviewedId, profile.Homeowner.AverageRating);
        }
        else if (profile?.Company is not null)
        {
            logger.LogInformation(
                "[Profiles] Company review received — rating not tracked for Company yet");
        }
        else
        {
            logger.LogWarning(
                "[Profiles] No profile found for ReviewedId={ReviewedId}",
                @event.ReviewedId);
        }
    }
}
