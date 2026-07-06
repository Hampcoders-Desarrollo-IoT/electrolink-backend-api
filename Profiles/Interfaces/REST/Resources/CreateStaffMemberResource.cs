namespace Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Resources;

public record CreateStaffMemberResource(
    string UserId,
    string FirstName,
    string LastName,
    string? PhoneNumber
);
