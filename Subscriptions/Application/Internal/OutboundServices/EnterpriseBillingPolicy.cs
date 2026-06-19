using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Services;

namespace Hampcoders.Electrolink.API.Subscriptions.Application.Internal.OutboundServices;

public class EnterpriseBillingPolicy : IBillingPolicy
{
    public UsageCounters? InitializeCounters()
        => null;

    public int? InitializeActiveDeviceCount()
        => 0;

    public void ProcessIncrement(Subscription subscription)
    {
    }

    public void ProcessReset(Subscription subscription)
    {
    }
}
