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
    public async Task<bool> CanCreateRequestAsync(string homeownerId)
        => (await subscriptionQueryService.Handle(new GetRequestEligibilityQuery(homeownerId))).CanRequest;

    public async Task<bool> CanMarkAsPriorityAsync(string homeownerId)
        => (await subscriptionQueryService.Handle(new GetRequestEligibilityQuery(homeownerId))).IsPriorityAllowed;

    public async Task<int?> GetRemainingRequestsAsync(string homeownerId)
        => (await subscriptionQueryService.Handle(new GetRequestEligibilityQuery(homeownerId))).RemainingRequests;

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
        GetRequestEligibilityAsync(string homeownerId)
    {
        var eligibility = await subscriptionQueryService.Handle(new GetRequestEligibilityQuery(homeownerId));
        return (
            eligibility.CanRequest,
            eligibility.PlanType,
            eligibility.RemainingRequests,
            eligibility.IsPriorityAllowed);
    }

    public async Task<bool> IncrementMonthlyRequestUsageAsync(string homeownerId)
    {
        if (string.IsNullOrWhiteSpace(homeownerId))
            return false;

        try
        {
            var profileId = await profilesContextFacade.GetProfileIdByHomeownerIdAsync(homeownerId);
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