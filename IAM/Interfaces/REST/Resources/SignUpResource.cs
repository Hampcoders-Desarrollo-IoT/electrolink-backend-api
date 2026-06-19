namespace Hampcoders.Electrolink.API.IAM.Interfaces.REST.Resources;

public record SignUpResource(string Email, string Password, string PasswordConfirmation, string Role = "User");