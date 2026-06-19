using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Commands;

namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Services;

public interface ISubscriptionCommandService
{
    Task<Subscription> Handle(CreateSubscriptionCommand command);
    Task<InitiateCheckoutResult> Handle(InitiateCheckoutCommand command);
    Task<Subscription?> Handle(ActivateSubscriptionCommand command);
    Task<Subscription?> Handle(ActivateEnterpriseSubscriptionPendingInstallationCommand command);
    Task<Subscription?> Handle(RecordSuccessfulRenewalCommand command);
    Task<Subscription> Handle(StartGracePeriodCommand command);
    Task<Subscription> Handle(DegradeSubscriptionCommand command);
    Task<Subscription> Handle(CancelSubscriptionCommand command);
    Task<Subscription> Handle(IncrementMonthlyRequestCounterCommand command);
    Task Handle(ResetMonthlyRequestCountersCommand command);
    Task<Subscription> Handle(UpdateBillingCycleCommand command);
    Task<CustomerPortalResult> Handle(OpenCustomerPortalCommand command);
    Task<Subscription> Handle(CancelEnterpriseSubscriptionWithRefundCommand command);
    Task<Subscription> Handle(UpdateActiveDeviceCountCommand command);
    Task Handle(RecordInstallationServiceRequestCommand command);
}

public record CustomerPortalResult(string PortalUrl);
