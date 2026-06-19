namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Commands;

/// <summary>
/// Command: Schedule voluntary cancellation at period end.
/// User action via dashboard/API.
/// </summary>
public record CancelSubscriptionCommand(
    string ProfileId,
    string Reason,
    string? Feedback);
