using Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Shared.Infrastructure;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Services;

/// <summary>
/// Command service interface for creating profiles (Homeowners or Technicians).
/// </summary>

public interface IProfileCommandService
{
    Task<Profile?> Handle(CreateProfileCommand command);
    Task<bool> Handle(UpdateTechnicianSpecialtiesCommand command);
    Task<Profile> Handle(CompleteProfileAsTechnicianCommand command);
    Task<Profile> Handle(CompleteProfileAsHomeownerCommand command);
    Task<Profile> Handle(CompleteProfileAsCompanyCommand command);
    Task<Profile> Handle(UpdateProfilePersonalDataCommand command);
    Task<Profile> Handle(UpdateTechnicianDataCommand command);
    Task<Profile> Handle(UpdateCommunicationPreferencesCommand command);
    Task<Profile> Handle(UploadProfilePictureCommand command);
    Task<SignedUploadData> Handle(GetProfilePhotoUploadUrlCommand command);
    Task<Profile> Handle(UpdateProfilePhotoCommand command);
    Task Handle(RemoveProfilePhotoCommand command);
    Task Handle(DeactivateProfileCommand command);
    Task Handle(ReactivateProfileCommand command);
    Task Handle(GrantIoTCertificationCommand command);
    Task Handle(RevokeIoTCertificationCommand command);
}
