using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Planning.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;

namespace Hampcoders.Electrolink.API.Planning.Application.Internal.CommandServices;

public class ServiceSuggestionCommandService(
    IServiceSuggestionRepository suggestionRepository,
    IServiceRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    ILogger<ServiceSuggestionCommandService> logger)
    : IServiceSuggestionCommandService
{
    public async Task<string> Handle(AcceptServiceSuggestionCommand command)
    {
        var suggestion = await suggestionRepository.FindBySuggestionIdAsync(command.SuggestionId)
            ?? throw new ArgumentException($"Suggestion {command.SuggestionId} not found.");

        if (suggestion.ClientId != command.Client.ClientId)
            throw new UnauthorizedAccessException("Suggestion does not belong to this client.");

        var serviceRequest = ServiceRequest.Initiate(command.Client, canMarkAsPriority: false, remainingRequests: null);
        serviceRequest.SelectProperty(
            Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects.PropertyId.From(suggestion.PropertyId),
            null!);

        var requestId = serviceRequest.RequestId.Value;
        suggestion.Accept(requestId);
        suggestionRepository.Update(suggestion);
        await requestRepository.AddAsync(serviceRequest);
        await unitOfWork.CompleteAsync();

        logger.LogInformation(
            "[Planning] Suggestion {SuggestionId} accepted -> ServiceRequest {RequestId}",
            suggestion.SuggestionId, requestId);

        return requestId;
    }

    public async Task Handle(DismissServiceSuggestionCommand command)
    {
        var suggestion = await suggestionRepository.FindBySuggestionIdAsync(command.SuggestionId)
            ?? throw new ArgumentException($"Suggestion {command.SuggestionId} not found.");

        if (suggestion.ClientId != command.Client.ClientId)
            throw new UnauthorizedAccessException("Suggestion does not belong to this client.");

        suggestion.Expire();
        suggestionRepository.Update(suggestion);
        await unitOfWork.CompleteAsync();

        logger.LogInformation(
            "[Planning] Suggestion {SuggestionId} dismissed by client {ClientId}",
            suggestion.SuggestionId, command.Client.ClientId);
    }
}
