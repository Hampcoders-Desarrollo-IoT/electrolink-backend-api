using Hampcoders.Electrolink.API.Assets.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Assets.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Assets.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hampcoders.Electrolink.API.Assets.Infrastructure.Persistence.EFC.Repositories;

public class PropertyPortfolioRepository(AppDbContext context)
    : BaseRepository<PropertyPortfolio, PropertyPortfolioId>(context), IPropertyPortfolioRepository
{
    public async Task<PropertyPortfolio?> FindByOwnerAsync(ClientIdentity owner)
        => await Context.Set<PropertyPortfolio>()
            .FirstOrDefaultAsync(pp => pp.Owner.ClientType == owner.ClientType && pp.Owner.ClientId == owner.ClientId);

    public async Task<PropertyPortfolio?> FindByOwnerWithEntriesAsync(ClientIdentity owner)
        => await Context.Set<PropertyPortfolio>()
            .Include(pp => pp.Entries)
            .FirstOrDefaultAsync(pp => pp.Owner.ClientType == owner.ClientType && pp.Owner.ClientId == owner.ClientId);
}