using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Events;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Profiles.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Application.Internal.EventHandler;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;

namespace Hampcoders.Electrolink.API.Profiles.Application.Internal.EventHandlers;

public class ServiceCompletedEventHandler(
    IProfileRepository profileRepository,
    IUnitOfWork unitOfWork,
    ILogger<ServiceCompletedEventHandler> logger)
    : IEventHandler<ServiceCompletedEvent>
{
    public async Task Handle(ServiceCompletedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Profiles] Service completed by Technician={TechnicianId}, adding to portfolio",
            @event.TechnicianId);

        var profile = await profileRepository.FindByTechnicianIdAsync(@event.TechnicianId);

        if (profile?.Technician is null)
        {
            logger.LogWarning("[Profiles] No technician found for TechnicianId={TechnicianId}",
                @event.TechnicianId);
            return;
        }

        var portfolioItem = new PortfolioItem(
            PropertyId: @event.PropertyId.Value,
            WorkSummary: @event.WorkSummary,
            ServiceCategory: @event.ServiceCategory,
            PhotoUrls: @event.PhotosUrls,
            CompletedAt: @event.CompletedAt);

        profile.Technician.AddPortfolioItem(portfolioItem);
        profileRepository.Update(profile);
        await unitOfWork.CompleteAsync();

        logger.LogInformation(
            "[Profiles] Portfolio item added for Technician {TechnicianId}",
            @event.TechnicianId);
    }
}
