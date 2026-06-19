using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;

namespace Hampcoders.Electrolink.API.Planning.Domain.Repositories;

public interface IServiceSuggestionRepository
{
    Task AddAsync(ServiceSuggestion suggestion);
    void Update(ServiceSuggestion suggestion);
    Task<ServiceSuggestion?> FindBySuggestionIdAsync(string suggestionId);
    Task<IEnumerable<ServiceSuggestion>> FindByClientIdAsync(string clientId);
    Task<IEnumerable<ServiceSuggestion>> FindActiveByClientIdAsync(string clientId);
    Task<IEnumerable<ServiceSuggestion>> FindPendingExpiredAsync(DateTime threshold);
}
