namespace Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Resources;

public record MyProfileResource(
    string ProfileId,
    string UserId,
    string Status,
    string? BusinessRole,
    string? ProfilePictureUrl,
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? Dni,
    string? DateOfBirth,
    string? Street,
    string? District,
    string? City,
    string? Country,
    string? PostalCode,
    TechnicianProfileResource? Technician,
    HomeownerProfileResource? Homeowner,
    CompanyProfileResource? Company);