namespace Hampcoders.Electrolink.API.IAM.Domain.Model.Commands;

public record SuspendUserAccountCommand(string UserId, string Reason);
