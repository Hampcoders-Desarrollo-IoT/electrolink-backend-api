using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Planning.Domain.Services;

namespace Hampcoders.Electrolink.API.Planning.Application.Internal.QueryServices;

public class ServiceSuggestionQueryService(IServiceSuggestionRepository suggestionRepository)
    : IServiceSuggestionQueryService
{
    public async Task<IEnumerable<ServiceSuggestion>> GetActiveByClientIdAsync(string clientId)
        => await suggestionRepository.FindActiveByClientIdAsync(clientId);
}
