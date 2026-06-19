using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Services;

public interface IBillingPolicy
{
    UsageCounters? InitializeCounters();
    int? InitializeActiveDeviceCount();
    void ProcessIncrement(Subscription subscription);
    void ProcessReset(Subscription subscription);
}
