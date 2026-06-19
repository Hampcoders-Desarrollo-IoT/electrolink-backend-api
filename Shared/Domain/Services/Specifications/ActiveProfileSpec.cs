using System.Linq.Expressions;
using Hampcoders.Electrolink.API.Profiles.Interfaces.ACL;

namespace Hampcoders.Electrolink.API.Shared.Domain.Services.Specifications;

public class ActiveProfileSpec : ISpecification<string>
{
    private readonly IProfilesContextFacade _profiles;

    public ActiveProfileSpec(IProfilesContextFacade profiles)
    {
        _profiles = profiles;
    }

    public bool IsSatisfiedBy(string userId)
    {
        var claims = _profiles.GetProfileClaimsAsync(userId).GetAwaiter().GetResult();
        return claims?.ProfileStatus == "Active";
    }

    public async Task<bool> IsSatisfiedByAsync(string userId)
    {
        var claims = await _profiles.GetProfileClaimsAsync(userId);
        return claims?.ProfileStatus == "Active";
    }

    public Expression<Func<string, bool>> ToExpression()
        => userId => _profiles.GetProfileClaimsAsync(userId).GetAwaiter().GetResult().GetValueOrDefault().ProfileStatus == "Active";
}
