namespace Hampcoders.Electrolink.API.IAM.Interfaces.REST.Resources;

public record CreateStaffUserResource(
    string Email,
    string Password,
    string PasswordConfirmation,
    string AccessRole,
    string FirstName,
    string LastName,
    string? PhoneNumber
);
