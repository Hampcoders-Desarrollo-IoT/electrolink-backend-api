namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Commands;

public record CreateStaffMemberCommand(
    string UserId,
    string FirstName,
    string LastName,
    string? PhoneNumber
);
