
using Hampcoders.Electrolink.API.Profiles.Domain.Model.Entities;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.Events;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.Exceptions;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Profiles.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;

public partial class Profile : BaseAggregateRoot, ICompletable
{
    // ── Identity ──────────────────────────────────────────
    public ProfileId ProfileId { get; protected set; }
    public UserId UserId { get; private set; }

    // ── States ────────────────────────────────────────────
    public EProfileStatus Status { get; private set; }
    public EBusinessRole? BusinessRole { get; private set; }
    public string? SubscriptionTier { get; private set; }

    // ── Personal Data ─────
    public PersonalData? PersonalData { get; private set; }
    
    // ── Profile Picture ──
    public ProfilePhoto? Photo { get; private set; }

    // ── Sub-entities according to role ──
    public Technician? Technician { get; private set; }
    public HomeOwner? Homeowner { get; private set; }
    public Company? Company { get; private set; }

    private Profile()
    {
    }

    // ── Factory Method ────────────────────────────────────
    public static Profile Create(UserId userId)
    {
        var profile = new Profile
        {
            ProfileId = ProfileId.NewProfileId(),
            UserId = userId,
            Status = EProfileStatus.Incomplete,
            BusinessRole = null,
            PersonalData = null,
            Technician = null,
            Homeowner = null,
            Company = null,
        };

        profile.RaiseDomainEvent(new ProfileCreatedAsIncompleteEvent(
            profile.ProfileId, profile.UserId));

        return profile;
    }

    // ── Template Method: CompleteProfileCore ──────────────
    private async Task CompleteProfileCoreAsync(
        EBusinessRole role,
        PersonalData? personalData,
        IProfileUniquenessChecker uniquenessChecker,
        Func<Task> uniquenessValidation,
        Action roleEntityFactory,
        object subjectId)
    {
        EnsureStatus(EProfileStatus.Incomplete);
        await uniquenessValidation();
        PersonalData = personalData;
        BusinessRole = role;
        roleEntityFactory();
        Status = EProfileStatus.Active;

        RaiseDomainEvent(new ProfileCompletedEvent(
            ProfileId.Value,
            UserId.Value,
            subjectId,
            role,
            DateTime.UtcNow));
    }

    // ── Completion Methods ────────────────────────────────
    public async Task CompleteAsTechnician(
        PersonalData personalData,
        TechnicianData technicianData,
        IProfileUniquenessChecker uniquenessChecker)
    {
        var technicianId = TechnicianId.NewTechnicianId();

        await CompleteProfileCoreAsync(
            EBusinessRole.Technician,
            personalData,
            uniquenessChecker,
            uniquenessValidation: () => uniquenessChecker.EnsureDniIsUniqueAsync(personalData.Dni, ProfileId),
            roleEntityFactory: () =>
            {
                Technician = Technician.Create(
                    technicianId, ProfileId,
                    technicianData.Specialties,
                    technicianData.ExperienceYears,
                    technicianData.AboutMe,
                    technicianData.ServiceArea);
            },
            subjectId: technicianId.Value);
    }

    public async Task CompleteAsHomeowner(
        PersonalData personalData,
        HomeownerData homeownerData,
        IProfileUniquenessChecker uniquenessChecker)
    {
        var homeownerId = HomeownerId.NewHomeownerId();

        await CompleteProfileCoreAsync(
            EBusinessRole.HomeOwner,
            personalData,
            uniquenessChecker,
            uniquenessValidation: () => uniquenessChecker.EnsureDniIsUniqueAsync(personalData.Dni, ProfileId),
            roleEntityFactory: () =>
            {
                Homeowner = HomeOwner.Create(
                    homeownerId, ProfileId,
                    homeownerData.PreferredContactTime,
                    homeownerData.CommunicationPreferences,
                    homeownerData.EmergencyContact);
            },
            subjectId: homeownerId.Value);
    }

    public async Task CompleteAsCompany(
        CompanyData companyData,
        IProfileUniquenessChecker uniquenessChecker,
        PersonalData? personalData = null)
    {
        var companyId = CompanyId.NewCompanyId();

        await CompleteProfileCoreAsync(
            EBusinessRole.Company,
            personalData,
            uniquenessChecker,
            uniquenessValidation: () => uniquenessChecker.EnsureTaxIdIsUniqueAsync(companyData.TaxId, ProfileId),
            roleEntityFactory: () =>
            {
                Company = Company.Create(companyId, ProfileId, companyData);
            },
            subjectId: companyId.Value);
    }
    
    // ── Photo Management ──────────────────────────────────
    public void UpdateProfilePhoto(ProfilePhoto newPhoto)
    {
        if (newPhoto is null)
            throw new ArgumentNullException(nameof(newPhoto));

        Photo = newPhoto;
        RaiseDomainEvent(new ProfilePhotoUpdatedEvent(ProfileId, newPhoto.PublicUrl, newPhoto.ProviderId));
    }

    public string? RemoveProfilePhoto()
    {
        if (Photo is null)
            return null;

        var oldProviderId = Photo.ProviderId;
        Photo = null;
        RaiseDomainEvent(new ProfilePhotoRemovedEvent(ProfileId, oldProviderId));
        return oldProviderId;
    }

    // ── Subscription Tier ─────────────────────────────────
    public void UpdateSubscriptionTier(string tier)
    {
        SubscriptionTier = tier;
        RaiseDomainEvent(new ProfileSubscriptionTierChangedEvent(
            ProfileId, UserId, tier, DateTime.UtcNow));
    }

    // ── Personal Data Updates ─────────────────────────────
    public void UpdatePersonalData(string? firstName, string? lastName, PhoneNumber? phone, Address? address)
    {
        EnsureStatus(EProfileStatus.Active);
        PersonalData = PersonalData!.Update(firstName, lastName, phone, address);
        RaiseDomainEvent(new ProfilePersonalDataUpdatedEvent(
            ProfileId,
            firstName,
            lastName,
            phone?.Value));
    }

    // ── IoT Certification ─────────────────────────────────
    public void GrantIoTCertification(CertificationData certification)
    {
        EnsureStatus(EProfileStatus.Active);
        EnsureRole(EBusinessRole.Technician);
        Technician!.GrantIoTCertification(certification);
        RaiseDomainEvent(new IoTCertificationGrantedEvent(
            ProfileId.Value, Technician.TechnicianId.Value, DateTime.UtcNow));
    }

    public void RevokeIoTCertification()
    {
        EnsureStatus(EProfileStatus.Active);
        EnsureRole(EBusinessRole.Technician);
        Technician!.RevokeIoTCertification();
        RaiseDomainEvent(new IoTCertificationRevokedEvent(
            ProfileId.Value, Technician.TechnicianId.Value, DateTime.UtcNow));
    }

    // ── Role-specific Data Updates ────────────────────────
    public void UpdateTechnicianData(IEnumerable<ESpecialty>? specialties, int? experienceYears, string? aboutMe)
    {
        EnsureStatus(EProfileStatus.Active);
        EnsureRole(EBusinessRole.Technician);

        var specialtiesList = specialties?.ToList();

        if (specialtiesList is not null)
            Technician!.UpdateSpecialties(specialtiesList);

        if (experienceYears.HasValue)
            Technician!.UpdateExperienceYears(experienceYears.Value);
        
        if (aboutMe is not null)
            Technician!.UpdateAboutMe(aboutMe);

        RaiseDomainEvent(new TechnicianDataUpdatedEvent(
            ProfileId,
            specialtiesList,
            experienceYears,
            aboutMe,
            Technician!.TechnicianId));
    }
    
    public void UpdateHomeOwnerData(EContactTime? preferredContactTime, CommunicationPreferences? preferences, EmergencyContact? emergencyContact)
    {
        EnsureStatus(EProfileStatus.Active);
        EnsureRole(EBusinessRole.HomeOwner);

        if (preferences != null)
            Homeowner!.UpdateCommunicationPreferences(preferences);

        if (preferredContactTime != null)
            Homeowner!.UpdatePreferredContactTime(preferredContactTime.Value);

        if (emergencyContact != null)
            Homeowner!.UpdateEmergencyContact(emergencyContact);

        RaiseDomainEvent(new HomeownerDataUpdatedEvent(
            ProfileId,
            Homeowner!.HomeownerId,
            preferredContactTime,
            preferences,
            emergencyContact));
    }

    public void UpdateCompanyData(CompanyData data)
    {
        EnsureStatus(EProfileStatus.Active);
        EnsureRole(EBusinessRole.Company);

        Company!.UpdateCompanyData(data);

        RaiseDomainEvent(new CompanyDataUpdatedEvent(
            ProfileId,
            Company!.CompanyId,
            data.CompanyName,
            data.Industry));
    }

    // ── State Transitions (ICompletable) ──────────────────
    public void Deactivate()
    {
        EnsureStatus(EProfileStatus.Active);
        
        Status = EProfileStatus.Deactivated;
        RaiseDomainEvent(new ProfileDeactivatedEvent(ProfileId, UserId, BusinessRole!.Value));
    }
    
    public void Reactivate()
    {
        EnsureStatus(EProfileStatus.Deactivated);
        Status = EProfileStatus.Active;
        RaiseDomainEvent(new ProfileReactivatedEvent(ProfileId, UserId, BusinessRole!.Value));
    }
    
    public void Suspend()
    {
        EnsureStatus(EProfileStatus.Active);
        
        Status = EProfileStatus.Suspended;
        RaiseDomainEvent(new ProfileSuspendedEvent(ProfileId, UserId, BusinessRole!.Value));
    }
    
    // ── Helper Methods for State Validation ────────────────
    private void EnsureStatus(EProfileStatus expected)
    {
        if (Status != expected)
            throw new InvalidProfileStatusException(ProfileId, expected, Status);
    }

    private void EnsureRole(EBusinessRole expected)
    {
        if (BusinessRole != expected)
            throw new InvalidBusinessRoleException(ProfileId, expected, BusinessRole);
    }
}

