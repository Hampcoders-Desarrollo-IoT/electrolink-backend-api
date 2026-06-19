using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Commands;

public record CompleteProfileAsCompanyCommand(
    string UserId,
    string CompanyName,
    string TaxId,
    string BillingStreet,
    string BillingNumber,
    string BillingDistrict,
    string BillingCity,
    string BillingCountry,
    string BillingPostalCode,
    string? Industry,
    ECompanySize CompanySize,
    string? Website,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? Dni,
    string? DateOfBirth,
    string? Street,
    string? Number,
    string? District,
    string? City,
    string? Country,
    string? PostalCode);
