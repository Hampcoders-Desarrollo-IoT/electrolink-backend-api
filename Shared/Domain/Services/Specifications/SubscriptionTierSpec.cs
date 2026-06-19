using System.Linq.Expressions;

namespace Hampcoders.Electrolink.API.Shared.Domain.Services.Specifications;

public class SubscriptionTierSpec : ISpecification<string>
{
    private readonly string[] _allowedTiers;
    private readonly ISubscriptionTierQuery _query;

    public SubscriptionTierSpec(ISubscriptionTierQuery query, params string[] allowedTiers)
    {
        _query = query;
        _allowedTiers = allowedTiers;
    }

    public bool IsSatisfiedBy(string userId)
    {
        var tier = _query.GetTierAsync(userId).GetAwaiter().GetResult();
        return tier is not null && _allowedTiers.Contains(tier);
    }

    public async Task<bool> IsSatisfiedByAsync(string userId)
    {
        var tier = await _query.GetTierAsync(userId);
        return tier is not null && _allowedTiers.Contains(tier);
    }

    public Expression<Func<string, bool>> ToExpression()
        => userId => _allowedTiers.Contains(_query.GetTierAsync(userId).GetAwaiter().GetResult());
}
