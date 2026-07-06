namespace Hampcoders.Electrolink.API.IAM.Domain.Model.Commands;

public record CreateStaffUserCommand(
    string Email,
    string Password,
    string PasswordConfirmation,
    string AccessRole,
    string FirstName,
    string LastName,
    string? PhoneNumber
);
