using Hampcoders.Electrolink.API.Assets.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Assets.Interfaces.REST.Resources;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST.Transform;

public static class PropertyPortfolioResourceFromEntityAssembler
{
    public static PropertyPortfolioResource ToResourceFromEntity(PropertyPortfolio portfolio)
    {
        var entries = portfolio.Entries
            .Select(e => new PortfolioEntryResource(
                e.PropertyId.Value,
                e.Nickname,
                e.IsPrimary,
                e.OccupancyStatus.ToString()))
            .ToList();

        return new PropertyPortfolioResource(
            portfolio.Id.Value,
            portfolio.Owner.ClientId,
            portfolio.Status.ToString(),
            entries);
    }
}

