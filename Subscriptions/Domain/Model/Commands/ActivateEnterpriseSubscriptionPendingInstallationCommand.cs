namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Commands;

public record ActivateEnterpriseSubscriptionPendingInstallationCommand(
    string StripeCustomerId,
    string StripeSubscriptionId,
    string StripeInvoiceId,
    int AmountPaid,
    string Currency,
    DateTime PeriodStart,
    DateTime PeriodEnd,
    int InitialDeviceCount,
    int PricePerDevice,
    string PlanType);
