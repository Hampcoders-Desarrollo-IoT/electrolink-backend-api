namespace Hampcoders.Electrolink.API.Profiles.Application.Internal.ReadModels;

public sealed record CompanyReadModel(
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
