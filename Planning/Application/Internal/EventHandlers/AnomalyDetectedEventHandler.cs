using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Processing.Domain.Model.Events;
using Hampcoders.Electrolink.API.Shared.Application.Internal.EventHandler;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;

namespace Hampcoders.Electrolink.API.Planning.Application.Internal.EventHandlers;

public class AnomalyDetectedEventHandler(
    IServiceSuggestionRepository suggestionRepository,
    IUnitOfWork unitOfWork,
    ILogger<AnomalyDetectedEventHandler> logger)
    : IEventHandler<AnomalyDetectedEvent>
{
    public async Task Handle(AnomalyDetectedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "[Planning] Anomaly detected — Device={DeviceId}, Type={AnomalyType}, Severity={Severity}",
            @event.DeviceId, @event.AnomalyType, @event.Severity);

        var expiresAt = DateTime.UtcNow.AddHours(48);
        var suggestion = ServiceSuggestion.Create(
            clientId: @event.OwnerId,
            profileId: null,
            propertyId: @event.PropertyId,
            anomalyId: @event.AnomalyId,
            deviceId: @event.DeviceId,
            anomalyType: @event.AnomalyType,
            severity: @event.Severity,
            expiresAt: expiresAt);

        await suggestionRepository.AddAsync(suggestion);
        await unitOfWork.CompleteAsync();

        logger.LogInformation(
            "[Planning] ServiceSuggestion {SuggestionId} created for Client={ClientId}",
            suggestion.SuggestionId, suggestion.ClientId);
    }
}
