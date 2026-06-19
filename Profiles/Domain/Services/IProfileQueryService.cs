using Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.ReadModels;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Services;

/// <summary>
/// Query service interface for retrieving profiles.
/// </summary>
public interface IProfileQueryService
{
  Task<Profile?> Handle(GetProfileByIdQuery query);
  Task<Profile?> Handle(GetProfileInfoByUserIdQuery query);
  Task<Profile?>Handle(GetMyProfileQuery query);
  Task<ProfileStatusReadModel?>Handle(GetProfileStatusQuery query);
  Task<bool>Handle(IsHomeownerActiveQuery query);
  Task<bool>Handle(IsCompanyActiveQuery query);
  Task<IEnumerable<(string technicianId, string profileId, string fullName)>> Handle(GetTechniciansInAreaQuery query);
  Task<(string ProfileId, string ProfileStatus, string? BusinessRole, string? RoleSubjectId, string? SubscriptionTier)?> Handle(GetProfileClaimsQuery query);
}
