using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Repository;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Services;

namespace Hampcoders.Electrolink.API.Subscriptions.Application.Internal.QueryServices;

public class SubscriptionQueryService(
    ISubscriptionRepository subscriptionRepository) : ISubscriptionQueryService
{
    public async Task<Subscription> Handle(GetMySubscriptionQuery query)
        => await subscriptionRepository.FindByProfileIdOrFailAsync(query.ProfileId);

    public async Task<RequestEligibilityResult> Handle(GetRequestEligibilityQuery query)
    {
        var subscription = await subscriptionRepository.FindByProfileIdOrFailAsync(query.ProfileId);

        if (subscription.PlanType.IsPremium)
        {
            return new RequestEligibilityResult(
                CanRequest: true,
                IsPriorityAllowed: true,
                RemainingRequests: null,
                PlanType: subscription.PlanType.ToString(),
                UpgradeRequired: false);
        }

        var hasCapacity = subscription.UsageCounters?.HasCapacity ?? false;
        return new RequestEligibilityResult(
            CanRequest: hasCapacity,
            IsPriorityAllowed: false,
            RemainingRequests: subscription.UsageCounters?.Remaining ?? 0,
            PlanType: subscription.PlanType.ToString(),
            UpgradeRequired: !hasCapacity);
    }

    public async Task<IEnumerable<PaymentRecord>> Handle(GetPaymentHistoryQuery query)
    {
        var subscription = await subscriptionRepository.FindByProfileIdOrFailAsync(query.ProfileId);
        return await subscriptionRepository.FindPaymentHistoryAsync(
            subscription.SubscriptionId.Value, query.Page, query.PageSize);
    }

    public async Task<Subscription?> Handle(GetSubscriptionStatusAlertQuery query)
    {
        var subscription = await subscriptionRepository.FindByProfileIdOrFailAsync(query.ProfileId);
        return subscription.Status.IsInGracePeriod ? subscription : null;
    }

    public async Task<Subscription?> Handle(GetSubscriptionByStripeCustomerIdQuery query)
        => await subscriptionRepository.FindByStripeCustomerIdAsync(query.StripeCustomerId);
}