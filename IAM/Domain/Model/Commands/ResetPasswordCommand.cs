namespace Hampcoders.Electrolink.API.IAM.Domain.Model.Commands;

public record ResetPasswordCommand(string ResetToken, string NewPassword, string NewPasswordConfirmation);
