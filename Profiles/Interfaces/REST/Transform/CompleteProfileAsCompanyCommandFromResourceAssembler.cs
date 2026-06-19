using Hampcoders.Electrolink.API.Profiles.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Transform;

public static class CompleteProfileAsCompanyCommandFromResourceAssembler
{
    public static CompleteProfileAsCompanyCommand ToCommandFromResource(
        CompleteProfileAsCompanyResource resource,
        string userId)
    {
        return new CompleteProfileAsCompanyCommand(
            UserId: userId,
            CompanyName: resource.CompanyName,
            TaxId: resource.TaxId,
            BillingStreet: resource.BillingStreet,
            BillingNumber: resource.BillingNumber,
            BillingDistrict: resource.BillingDistrict,
            BillingCity: resource.BillingCity,
            BillingCountry: resource.BillingCountry,
            BillingPostalCode: resource.BillingPostalCode,
            Industry: resource.Industry,
            CompanySize: resource.CompanySize,
            Website: resource.Website,
            FirstName: resource.FirstName,
            LastName: resource.LastName,
            PhoneNumber: resource.PhoneNumber,
            Dni: resource.Dni,
            DateOfBirth: resource.DateOfBirth,
            Street: resource.Street,
            Number: resource.Number,
            District: resource.District,
            City: resource.City,
            Country: resource.Country,
            PostalCode: resource.PostalCode);
    }
}
