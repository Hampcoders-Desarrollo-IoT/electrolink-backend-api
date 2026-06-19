using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Services;

namespace Hampcoders.Electrolink.API.Subscriptions.Application.Internal.OutboundServices;

public class HomeownerBillingPolicy : IBillingPolicy
{
    public UsageCounters? InitializeCounters()
        => UsageCounters.Initial();

    public int? InitializeActiveDeviceCount()
        => null;

    public void ProcessIncrement(Subscription subscription)
    {
        if (subscription.PlanType.IsBasic)
            subscription.IncrementRequestCounter();
    }

    public void ProcessReset(Subscription subscription)
    {
        subscription.ResetMonthlyCounters();
    }
}
