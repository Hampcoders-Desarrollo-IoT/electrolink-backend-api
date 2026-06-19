using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;

namespace Hampcoders.Electrolink.API.Planning.Infrastructure.BackgroundServices;

public class ServiceSuggestionExpirationJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ServiceSuggestionExpirationJob> _logger;

    public ServiceSuggestionExpirationJob(
        IServiceScopeFactory scopeFactory,
        ILogger<ServiceSuggestionExpirationJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[ServiceSuggestionExpiration] Background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExpirePendingSuggestionsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ServiceSuggestionExpiration] Error expiring suggestions");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ExpirePendingSuggestionsAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var suggestionRepository = scope.ServiceProvider.GetRequiredService<IServiceSuggestionRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var threshold = DateTime.UtcNow;
        var expired = await suggestionRepository.FindPendingExpiredAsync(threshold);

        foreach (var suggestion in expired)
        {
            suggestion.Expire();
        }

        if (expired.Any())
        {
            await unitOfWork.CompleteAsync();
            _logger.LogInformation("[ServiceSuggestionExpiration] Expired {Count} suggestions", expired.Count());
        }
    }
}
