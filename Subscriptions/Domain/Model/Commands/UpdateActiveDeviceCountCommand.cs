namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Commands;

public record UpdateActiveDeviceCountCommand(
    string SubscriptionId,
    int NewActiveDeviceCount,
    string Source);
