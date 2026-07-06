namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Commands;

public record UpdateStaffMemberDataCommand(
    string StaffMemberId,
    string FirstName,
    string LastName,
    string? PhoneNumber
);
