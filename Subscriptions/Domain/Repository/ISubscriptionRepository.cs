using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using UserId = Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects.UserId;
using ProfileId = Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects.ProfileId;

namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Repository;


/// <summary>
/// Repository interface for managing <see cref="Subscription"/> aggregates.
/// </summary>
public interface ISubscriptionRepository : IBaseRepository<Subscription, SubscriptionId>
{
/// <summary>
/// Finds a subscription by the user's unique identifier.
/// </summary>
/// <param name="userId">The user ID.</param>
/// <returns>The <see cref="Subscription"/> if found, otherwise null.</returns>
Task<Subscription?> FindByUserIdAsync(string userId);
    
Task<Subscription> FindByUserIdOrFailAsync(string userId);

/// <summary>
/// Finds a subscription by the profile's unique identifier.
/// </summary>
/// <param name="profileId">The profile ID.</param>
/// <returns>The <see cref="Subscription"/> if found, otherwise null.</returns>
Task<Subscription?> FindByProfileIdAsync(string profileId);

Task<Subscription> FindByProfileIdOrFailAsync(string profileId);

    /// <summary>
    /// Finds a subscription by its Stripe Customer ID.
    /// </summary>
    /// <param name="stripeCustomerId">The Stripe Customer ID.</param>
    /// <returns>The <see cref="Subscription"/> if found, otherwise null.</returns>
    Task<Subscription?> FindByStripeCustomerIdAsync(string stripeCustomerId);
    Task<Subscription> FindByStripeCustomerIdOrFailAsync(string stripeCustomerId);

    /// <summary>
    /// Finds a subscription by its Stripe Subscription ID.
    /// </summary>
    /// <param name="stripeSubscriptionId">The Stripe Subscription ID.</param>
    /// <returns>The <see cref="Subscription"/> if found, otherwise null.</returns>
    Task<Subscription?> FindByStripeSubscriptionIdAsync(string stripeSubscriptionId);
    Task<Subscription> FindByStripeSubscriptionIdOrFailAsync(string stripeSubscriptionId);

    Task<bool> ExistsByUserIdAsync(UserId userId);
    Task<bool> ExistsByProfileIdAsync(ProfileId profileId);
    Task<IEnumerable<Subscription>> FindAllInGracePeriodExpiredAsync(DateTime asOf, int limit = 100, int offset = 0);
    Task<IEnumerable<Subscription>> FindAllBasicHomeownersAsync(int limit = 100, int offset = 0);
    Task<IEnumerable<PaymentRecord>> FindPaymentHistoryAsync(string subscriptionId, int page = 1, int pageSize = 20);
    Task<IEnumerable<Subscription>> FindPendingInstallationExpiredAsync(DateTime threshold);
    Task<Subscription?> FindByPendingInstallationServiceRequestIdAsync(string serviceRequestId);
}
