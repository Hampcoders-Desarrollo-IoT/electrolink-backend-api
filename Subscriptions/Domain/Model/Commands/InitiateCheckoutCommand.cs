namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Commands;

public record InitiateCheckoutCommand(
    string ProfileId,
    string PlanType,
    string BillingCycle,
    string SuccessUrl,
    string CancelUrl);
