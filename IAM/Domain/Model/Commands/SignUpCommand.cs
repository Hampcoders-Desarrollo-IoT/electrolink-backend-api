namespace Hampcoders.Electrolink.API.IAM.Domain.Model.Commands;

public record SignUpCommand(string Email, string Password, string PasswordConfirmation, string Role);
