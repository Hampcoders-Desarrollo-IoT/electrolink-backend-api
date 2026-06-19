using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hampcoders.Electrolink.API.Planning.Infrastructure.Persistence.EFC.Repositories;

public class ServiceSuggestionRepository(AppDbContext context)
    : IServiceSuggestionRepository
{
    public async Task AddAsync(ServiceSuggestion suggestion)
        => await context.Set<ServiceSuggestion>().AddAsync(suggestion);

    public void Update(ServiceSuggestion suggestion)
        => context.Set<ServiceSuggestion>().Update(suggestion);

    public async Task<ServiceSuggestion?> FindBySuggestionIdAsync(string suggestionId)
        => await context.Set<ServiceSuggestion>().FirstOrDefaultAsync(s => s.SuggestionId == suggestionId);

    public async Task<IEnumerable<ServiceSuggestion>> FindByClientIdAsync(string clientId)
        => await context.Set<ServiceSuggestion>()
            .Where(s => s.ClientId == clientId)
            .OrderByDescending(s => s.ExpiresAt)
            .ToListAsync();

    public async Task<IEnumerable<ServiceSuggestion>> FindActiveByClientIdAsync(string clientId)
        => await context.Set<ServiceSuggestion>()
            .Where(s => s.ClientId == clientId
                     && s.Status == ESuggestionStatus.Pending)
            .OrderByDescending(s => s.ExpiresAt)
            .ToListAsync();

    public async Task<IEnumerable<ServiceSuggestion>> FindPendingExpiredAsync(DateTime threshold)
        => await context.Set<ServiceSuggestion>()
            .Where(s => s.Status == ESuggestionStatus.Pending
                     && s.ExpiresAt <= threshold)
            .OrderBy(s => s.ExpiresAt)
            .ToListAsync();
}
