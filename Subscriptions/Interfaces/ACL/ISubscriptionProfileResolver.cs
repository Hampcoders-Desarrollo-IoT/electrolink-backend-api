using Hampcoders.Electrolink.API.Subscriptions.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Subscriptions.Interfaces.ACL;

public interface ISubscriptionProfileResolver
{
    Task<BusinessRole> ResolveBusinessRoleAsync(string userId);

    Task<string?> ResolveProfileFullNameAsync(string userId);
}
