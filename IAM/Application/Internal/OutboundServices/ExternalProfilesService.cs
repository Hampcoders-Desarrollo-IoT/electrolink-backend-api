using Hampcoders.Electrolink.API.Profiles.Interfaces.ACL;

namespace Hampcoders.Electrolink.API.IAM.Application.Internal.OutboundServices;

public class ExternalProfilesService(IProfilesContextFacade profilesContextFacade)
{
    public async Task<(string ProfileId, string ProfileStatus, string? BusinessRole, string? RoleSubjectId, string? SubscriptionTier)?> GetProfileClaimsAsync(string userId) 
        => await profilesContextFacade.GetProfileClaimsAsync(userId);
}