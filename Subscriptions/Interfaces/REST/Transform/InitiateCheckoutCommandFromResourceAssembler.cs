using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Subscriptions.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.Subscriptions.Interfaces.REST.Transform;

public static class InitiateCheckoutCommandFromResourceAssembler
{
    public static InitiateCheckoutCommand ToCommand(string profileId, InitiateCheckoutResource resource)
        => new(
            ProfileId: profileId,
            PlanType: resource.PlanType,
            BillingCycle: resource.BillingCycle,
            SuccessUrl: resource.SuccessUrl,
            CancelUrl: resource.CancelUrl);
}

