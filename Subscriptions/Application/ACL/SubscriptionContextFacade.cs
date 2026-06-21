using Hampcoders.Electrolink.API.Profiles.Interfaces.ACL;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Subscriptions.Domain.Services;
using Hampcoders.Electrolink.API.Subscriptions.Interfaces.ACL;

namespace Hampcoders.Electrolink.API.Subscriptions.Application.ACL;

public class SubscriptionContextFacade(
    ISubscriptionQueryService subscriptionQueryService,
    ISubscriptionCommandService subscriptionCommandService,
    IProfilesContextFacade profilesContextFacade)
    : ISubscriptionContextFacade
{
    private async Task<string?> ResolveProfileIdAsync(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId)) return null;
        if (clientId.StartsWith("ho-"))
            return await profilesContextFacade.GetProfileIdByHomeownerIdAsync(clientId);
        if (clientId.StartsWith("comp-"))
            return await profilesContextFacade.GetProfileIdByCompanyIdAsync(clientId);
        return null;
    }

    public async Task<bool> CanCreateRequestAsync(string clientId)
    {
        var profileId = await ResolveProfileIdAsync(clientId);
        if (profileId is null) return false;
        return (await subscriptionQueryService.Handle(new GetRequestEligibilityQuery(profileId))).CanRequest;
    }

    public async Task<bool> CanMarkAsPriorityAsync(string clientId)
    {
        var profileId = await ResolveProfileIdAsync(clientId);
        if (profileId is null) return false;
        return (await subscriptionQueryService.Handle(new GetRequestEligibilityQuery(profileId))).IsPriorityAllowed;
    }

    public async Task<int?> GetRemainingRequestsAsync(string clientId)
    {
        var profileId = await ResolveProfileIdAsync(clientId);
        if (profileId is null) return null;
        return (await subscriptionQueryService.Handle(new GetRequestEligibilityQuery(profileId))).RemainingRequests;
    }

    public async Task<bool> IsTechnicianPremiumAsync(string technicianId)
    {
        try
        {
            var subscription = await subscriptionQueryService.Handle(new GetMySubscriptionQuery(technicianId));
            return subscription.PlanType.IsPremium && subscription.Status.IsActive;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public async Task<(bool canCreate, string planType, int? remainingRequests, bool canMarkAsPriority)>
        GetRequestEligibilityAsync(string clientId)
    {
        var profileId = await ResolveProfileIdAsync(clientId);
        if (profileId is null)
            return (false, "UNKNOWN", null, false);

        var eligibility = await subscriptionQueryService.Handle(new GetRequestEligibilityQuery(profileId));
        return (
            eligibility.CanRequest,
            eligibility.PlanType,
            eligibility.RemainingRequests,
            eligibility.IsPriorityAllowed);
    }

    public async Task<bool> IncrementMonthlyRequestUsageAsync(string clientId)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            return false;

        try
        {
            var profileId = await ResolveProfileIdAsync(clientId);
            if (string.IsNullOrWhiteSpace(profileId))
                return false;

            await subscriptionCommandService.Handle(new IncrementMonthlyRequestCounterCommand(profileId));
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public Task<bool> TechnicianHasPremiumSubscriptionAsync(string technicianId)
        => IsTechnicianPremiumAsync(technicianId);

    public async Task RecordInstallationServiceRequestAsync(string subscriptionId, string serviceRequestId)
    {
        try
        {
            await subscriptionCommandService.Handle(
                new RecordInstallationServiceRequestCommand(subscriptionId, serviceRequestId));
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<string?> GetTechnicianPlanTypeAsync(string technicianId)
    {
        try
        {
            var subscription = await subscriptionQueryService.Handle(new GetMySubscriptionQuery(technicianId));
            return subscription.PlanType.ToString();
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}