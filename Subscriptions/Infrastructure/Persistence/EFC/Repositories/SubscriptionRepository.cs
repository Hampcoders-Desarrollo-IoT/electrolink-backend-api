using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Repository;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;
using ProfileId = Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects.ProfileId;

namespace Hampcoders.Electrolink.API.Subscriptions.Infrastructure.Persistence.EFC.Repositories;

public class SubscriptionRepository(AppDbContext context)
    : BaseRepository<Subscription, SubscriptionId>(context), ISubscriptionRepository
{
    public async Task<Subscription?> FindByUserIdAsync(string userId)
    {
        var id = UserId.From(userId);
        return await Context.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.UserId == id);
    }

    public async Task<Subscription> FindByUserIdOrFailAsync(string userId)
        => await FindByUserIdAsync(userId)
           ?? throw new ArgumentException($"No subscription found for user {userId}.");

    public async Task<Subscription?> FindByStripeCustomerIdAsync(string stripeCustomerId)
    {
        var id = StripeCustomerId.From(stripeCustomerId);
        return await Context.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.StripeCustomerId == id);
    }

    public async Task<Subscription> FindByStripeCustomerIdOrFailAsync(string stripeCustomerId)
        => await FindByStripeCustomerIdAsync(stripeCustomerId)
           ?? throw new ArgumentException($"No subscription found for Stripe customer {stripeCustomerId}.");

    public async Task<Subscription?> FindByStripeSubscriptionIdAsync(string stripeSubscriptionId)
    {
        var id = StripeSubscriptionId.From(stripeSubscriptionId);
        return await Context.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == id);
    }

    public async Task<Subscription> FindByStripeSubscriptionIdOrFailAsync(string stripeSubscriptionId)
        => await FindByStripeSubscriptionIdAsync(stripeSubscriptionId)
           ?? throw new ArgumentException($"No subscription found for Stripe subscription {stripeSubscriptionId}.");

    public async Task<bool> ExistsByUserIdAsync(UserId userId)
        => await Context.Set<Subscription>().AnyAsync(s => s.UserId == userId);

    public async Task<Subscription?> FindByProfileIdAsync(string profileId)
    {
        var id = ProfileId.From(profileId);
        return await Context.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.ProfileId == id);
    }

    public async Task<Subscription> FindByProfileIdOrFailAsync(string profileId)
        => await FindByProfileIdAsync(profileId)
           ?? throw new ArgumentException($"No subscription found for profile {profileId}.");

    public async Task<bool> ExistsByProfileIdAsync(ProfileId profileId)
        => await Context.Set<Subscription>().AnyAsync(s => s.ProfileId == profileId);

    public async Task<IEnumerable<Subscription>> FindAllInGracePeriodExpiredAsync(DateTime asOf, int limit = 100, int offset = 0)
        => await Context.Set<Subscription>()
            .Where(s => s.Status == SubscriptionStatus.GracePeriod && s.GracePeriodEndsAt != null && s.GracePeriodEndsAt <= asOf)
            .OrderBy(s => s.GracePeriodEndsAt)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

    public async Task<IEnumerable<Subscription>> FindAllBasicHomeownersAsync(int limit = 100, int offset = 0)
        => await Context.Set<Subscription>()
            .Where(s => s.PlanType == PlanType.Basic && s.BusinessRole == BusinessRole.Homeowner)
            .OrderBy(s => s.SubscriptionId)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

    public async Task<IEnumerable<Subscription>> FindPendingInstallationExpiredAsync(DateTime threshold)
        => await Context.Set<Subscription>()
            .Where(s => s.Status == SubscriptionStatus.PendingInstallation
                     && s.InstallationDeadlineAt != null
                     && s.InstallationDeadlineAt <= threshold)
            .OrderBy(s => s.InstallationDeadlineAt)
            .ToListAsync();

    public async Task<Subscription?> FindByPendingInstallationServiceRequestIdAsync(string serviceRequestId)
        => await Context.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.InstallationServiceRequestId == serviceRequestId
                                   && s.Status == SubscriptionStatus.PendingInstallation);

    public async Task<IEnumerable<PaymentRecord>> FindPaymentHistoryAsync(string subscriptionId, int page = 1, int pageSize = 20)
    {
        var id = SubscriptionId.From(subscriptionId);
        return await Context.Set<PaymentRecord>()
            .Where(p => p.SubscriptionId == id)
            .OrderByDescending(p => p.ProcessedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }
}