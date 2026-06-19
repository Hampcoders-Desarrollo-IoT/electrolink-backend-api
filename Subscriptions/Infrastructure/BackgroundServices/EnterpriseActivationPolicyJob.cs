using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Repository;

namespace Hampcoders.Electrolink.API.Subscriptions.Infrastructure.BackgroundServices;

public class EnterpriseActivationPolicyJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EnterpriseActivationPolicyJob> _logger;

    public EnterpriseActivationPolicyJob(
        IServiceScopeFactory scopeFactory,
        ILogger<EnterpriseActivationPolicyJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[EnterpriseActivationPolicy] Background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckExpiredInstallationsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EnterpriseActivationPolicy] Error checking expired installations");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task CheckExpiredInstallationsAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var threshold = DateTime.UtcNow;
        var expired = await subscriptionRepository.FindPendingInstallationExpiredAsync(threshold);

        foreach (var subscription in expired)
        {
            try
            {
                _logger.LogWarning(
                    "[EnterpriseActivationPolicy] Installation expired for Sub {SubId}, cancelling with refund",
                    subscription.SubscriptionId.Value);

                subscription.CancelWithRefund("system", "Enterprise activation policy: 30-day installation deadline exceeded");
                await unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[EnterpriseActivationPolicy] Failed to cancel sub {SubId}",
                    subscription.SubscriptionId.Value);
            }
        }
    }
}
