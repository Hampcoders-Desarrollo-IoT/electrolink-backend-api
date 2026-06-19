namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Commands;

/// <summary>
/// Command: Increment monthly request counter for BASIC HOMEOWNER.
/// Called by ServiceRequestCreatedEventHandler when Planning BC publishes ServiceRequestCreated.
/// </summary>
public record IncrementMonthlyRequestCounterCommand(
    string ProfileId);
