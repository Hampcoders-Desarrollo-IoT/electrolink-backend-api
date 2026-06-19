using Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.ReadModels;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Profiles.Domain.Repositories;
using Hampcoders.Electrolink.API.Profiles.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Profiles.Application.Internal.QueryServices;

/// <summary>
/// Application-level query service for Profiles.
/// </summary>
public class ProfileQueryService(IProfileRepository profileRepository) : IProfileQueryService
{
    public async Task<Profile?> Handle(GetProfileByIdQuery query) => 
        await profileRepository.FindByIdAsync(ProfileId.From(query.ProfileId));
    

    public async Task<Profile?> Handle(GetProfileInfoByUserIdQuery query) =>
        await profileRepository.FindByUserIdAsync(query.UserId);

    public async Task<Profile?> Handle(GetMyProfileQuery query)
    {
        var profile = await profileRepository.FindByUserIdAsync(UserId.From(query.UserId));
        return profile;
    }

    public async Task<IEnumerable<(string technicianId, string profileId, string fullName)>> Handle(GetTechniciansInAreaQuery query)
        => await profileRepository.FindTechniciansInAreaAsync(query.Latitude, query.Longitude);

    public Task<(string ProfileId, string ProfileStatus, string? BusinessRole, string? RoleSubjectId, string? SubscriptionTier)?> Handle(GetProfileClaimsQuery query)
    {
        return profileRepository.FindProfileClaimsByUserIdAsync(UserId.From(query.UserId));
    }

    public async Task<ProfileStatusReadModel?> Handle(GetProfileStatusQuery query)
    {
        var profile = await profileRepository.FindByUserIdAsync(UserId.From(query.UserId));
        if (profile is null) return null;

        return new ProfileStatusReadModel(
            ProfileId: profile.ProfileId.Value,
            UserId: profile.UserId.Value,
            Status: profile.Status.ToString(),
            CompletionPercentage: CalculateCompletion(profile));
    }

    public async Task<bool> Handle(IsHomeownerActiveQuery query) => 
        await profileRepository.IsHomeownerActiveAsync(query.HomeownerId);
    
    public async Task<bool> Handle(IsCompanyActiveQuery query) => 
        await profileRepository.IsCompanyActiveAsync(query.CompanyId);
    
    private static int CalculateCompletion(Profile profile) =>
        profile.Status switch
        {
            EProfileStatus.Active => 100,
            EProfileStatus.Deactivated => 100,
            EProfileStatus.Incomplete => 50,
            EProfileStatus.Rejected => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(profile.Status), profile.Status, "Invalid profile status")
        };

}
