namespace Hampcoders.Electrolink.API.IAM.Interfaces.REST.Resources;

public record ResetPasswordResource(string ResetToken, string NewPassword, string NewPasswordConfirmation);
