using Hampcoders.Electrolink.API.Assets.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Assets.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;

namespace Hampcoders.Electrolink.API.Assets.Domain.Repositories;

public interface IPropertyPortfolioRepository : IBaseRepository<PropertyPortfolio, PropertyPortfolioId>
{
    Task<PropertyPortfolio?> FindByOwnerAsync(ClientIdentity owner);
    Task<PropertyPortfolio?> FindByOwnerWithEntriesAsync(ClientIdentity owner);

}