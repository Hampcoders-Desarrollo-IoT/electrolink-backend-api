using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;

namespace Hampcoders.Electrolink.API.Planning.Domain.Services;

public interface IServiceSuggestionQueryService
{
    Task<IEnumerable<ServiceSuggestion>> GetActiveByClientIdAsync(string clientId);
}
