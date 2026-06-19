using Hampcoders.Electrolink.API.Profiles.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.Entities;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Transform;

public class MyProfileResourceFromEntityAssembler
{
    public static MyProfileResource ToResourceFromEntity(Profile entity) => 
        new(
            ProfileId:    entity.ProfileId.Value,
            UserId:       entity.UserId.Value,
            Status:       entity.Status.ToString(),
            BusinessRole: entity.BusinessRole?.ToString(),
            ProfilePictureUrl: entity.Photo?.PublicUrl,
            FirstName:    entity.PersonalData?.FirstName,
            LastName:     entity.PersonalData?.LastName,
            PhoneNumber:  entity.PersonalData?.PhoneNumber.Value,
            Dni:          entity.PersonalData?.Dni.Value,
            DateOfBirth:  entity.PersonalData?.DateOfBirth.Value.ToString("yyyy-MM-dd"),
            Street:       entity.PersonalData?.Address?.Street,
            District:     entity.PersonalData?.Address?.District,
            City:         entity.PersonalData?.Address?.City,
            Country:      entity.PersonalData?.Address?.Country,
            PostalCode:   entity.PersonalData?.Address?.PostalCode,
            Technician:   entity.Technician is null ? null : MapTechnician(entity.Technician),
            Homeowner:    entity.Homeowner is null ? null : MapHomeowner(entity.Homeowner),
            Company:      entity.Company is null ? null : MapCompany(entity.Company));

    private static PersonalDataResource MapPersonalData(PersonalData pd) =>
        new(pd.FirstName,
            pd.LastName,
            pd.PhoneNumber.Value,
            pd.Dni.Value,
            pd.DateOfBirth.Value.ToString("yyyy-MM-dd"),
            pd.Address.Street,
            pd.Address.District,
            pd.Address.City,
            pd.Address.Country,
            pd.Address.PostalCode
            );

    private static TechnicianProfileResource MapTechnician(Technician t) =>
        new(t.TechnicianId.Value,
            t.Specialties.Select(s => s.ToString()).ToList(),
            t.ExperienceYears,
            t.AboutMe,
            t.ServiceArea.CenterLatitude,
            t.ServiceArea.CenterLongitude,
            t.ServiceArea.RadiusKm);

    private static HomeownerProfileResource MapHomeowner(HomeOwner h) =>
        new(h.HomeownerId.Value,
            h.PreferredContactTime.ToString(),
            h.CommunicationPreferences.SmsNotifications,
            h.CommunicationPreferences.EmailNotifications,
            h.CommunicationPreferences.PushNotifications,
            h.EmergencyContact?.Name,
            h.EmergencyContact?.Relationship,
            h.EmergencyContact?.PhoneNumber
            );

    private static CompanyProfileResource MapCompany(Company c) =>
        new(c.CompanyId.Value,
            c.CompanyData.CompanyName,
            c.CompanyData.TaxId.Value,
            c.CompanyData.Industry,
            c.CompanyData.Size.ToString(),
            c.CompanyData.Website,
            c.CompanyData.BillingAddress.Street,
            c.CompanyData.BillingAddress.Number,
            c.CompanyData.BillingAddress.District,
            c.CompanyData.BillingAddress.City,
            c.CompanyData.BillingAddress.Country,
            c.CompanyData.BillingAddress.PostalCode);
}