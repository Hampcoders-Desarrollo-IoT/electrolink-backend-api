namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Commands;

public record RecordInstallationServiceRequestCommand(
    string SubscriptionId,
    string ServiceRequestId);
