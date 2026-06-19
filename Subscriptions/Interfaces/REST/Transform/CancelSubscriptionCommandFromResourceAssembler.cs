using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Subscriptions.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.Subscriptions.Interfaces.REST.Transform;

public static class CancelSubscriptionCommandFromResourceAssembler
{
    public static CancelSubscriptionCommand ToCommand(string profileId, CancelSubscriptionResource resource)
        => new(
            ProfileId: profileId,
            Reason: resource.Reason,
            Feedback: resource.Feedback);
}