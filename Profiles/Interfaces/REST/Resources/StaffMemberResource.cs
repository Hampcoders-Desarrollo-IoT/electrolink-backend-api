namespace Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Resources;

public record StaffMemberResource(
    string Id,
    string UserId,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    bool IsIoTCertified,
    string? AssignedZone,
    bool IsActive,
    DateTime HireDate
);
