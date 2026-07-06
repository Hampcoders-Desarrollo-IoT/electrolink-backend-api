namespace Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Resources;

public record UpdateStaffMemberDataResource(
    string FirstName,
    string LastName,
    string? PhoneNumber
);
