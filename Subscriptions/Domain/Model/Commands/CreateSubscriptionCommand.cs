using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Model.Commands;

/// <summary>
/// Command: Create subscription for new user.
/// Called by InitializeSubscriptionForNewUser policy when ProfileCompleted event is received.
/// BusinessRole is resolved internally via ISubscriptionProfileResolver.
/// </summary>
public record CreateSubscriptionCommand(
    UserId UserId,
    ProfileId ProfileId);
