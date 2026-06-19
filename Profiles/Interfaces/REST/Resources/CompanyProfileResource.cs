namespace Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Resources;

public record CompanyProfileResource(
    string CompanyId,
    string CompanyName,
    string TaxId,
    string? Industry,
    string CompanySize,
    string? Website,
    string BillingStreet,
    string BillingNumber,
    string BillingDistrict,
    string BillingCity,
    string BillingCountry,
    string BillingPostalCode);
