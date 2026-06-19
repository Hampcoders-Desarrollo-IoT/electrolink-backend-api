using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Events;
using Hampcoders.Electrolink.API.Profiles.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Application.Internal.EventHandler;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Profiles.Application.Internal.EventHandlers;

public class TechnicianEvaluationSubmittedEventHandler(
    IProfileRepository profileRepository,
    IUnitOfWork unitOfWork,
    ILogger<TechnicianEvaluationSubmittedEventHandler> logger)
    : IEventHandler<TechnicianReviewSubmittedEvent>
{
    public async Task Handle(TechnicianReviewSubmittedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Profiles] Processing evaluation for Technician={TechnicianId}, Rating={Rating}",
            @event.ReviewedId, @event.Rating);

        var profile = await profileRepository.FindByTechnicianIdAsync(
            TechnicianId.From(@event.ReviewedId));

        if (profile?.Technician is null)
        {
            logger.LogWarning("[Profiles] No technician found for TechnicianId={TechnicianId}",
                @event.ReviewedId);
            return;
        }

        profile.Technician.UpdateRating(@event.Rating);
        profileRepository.Update(profile);
        await unitOfWork.CompleteAsync();

        logger.LogInformation(
            "[Profiles] Technician {TechnicianId} rating updated to {AverageRating}",
            @event.ReviewedId, profile.Technician.AverageRating);
    }
}
