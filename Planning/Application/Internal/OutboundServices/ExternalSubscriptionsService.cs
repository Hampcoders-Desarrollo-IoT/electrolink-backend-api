using Hampcoders.Electrolink.API.Subscriptions.Interfaces.ACL;

namespace Hampcoders.Electrolink.API.Planning.Application.Internal.OutboundServices;

/// <summary>
/// </summary>
public class ExternalSubscriptionsService(ISubscriptionContextFacade subscriptionContextFacade)
{
    /// <summary>
    /// </summary>
    public async Task<bool> CanCreateRequestAsync(string clientId)
    {
        var eligibility = await subscriptionContextFacade.GetRequestEligibilityAsync(clientId);
        return eligibility.canCreate;
    }

    /// <summary>
    /// </summary>
    public async Task<string> GetPlanTypeAsync(string clientId)
    {
        var eligibility = await subscriptionContextFacade.GetRequestEligibilityAsync(clientId);
        return eligibility.planType;
    }

    /// <summary>
    /// </summary>
    public async Task<(bool canCreate, string planType, int? remainingRequests, bool canMarkAsPriority)> GetRemainingRequestsAsync(string clientId) 
        => await subscriptionContextFacade.GetRequestEligibilityAsync(clientId);

    /// <summary>
    /// </summary>
    public async Task<bool> CanMarkAsPriorityAsync(string clientId)
    {
        var eligibility = await subscriptionContextFacade.GetRequestEligibilityAsync(clientId);
        return eligibility.canMarkAsPriority;
    }
}

