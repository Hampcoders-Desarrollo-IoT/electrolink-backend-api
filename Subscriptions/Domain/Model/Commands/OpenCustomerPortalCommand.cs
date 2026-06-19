namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Commands;

public record OpenCustomerPortalCommand(string ProfileId, string ReturnUrl);
