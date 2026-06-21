using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Entities;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using Hampcoders.Electrolink.API.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Hampcoders.Electrolink.API.Planning.Infrastructure.Persistence.EFC.Repositories;

public class ServiceCatalogRepository(AppDbContext context)  : BaseRepository<ServiceCatalog, CatalogId>(context), IServiceCatalogRepository
{
    public async Task<ServiceCatalog?> FindByTechnicianIdAsync(TechnicianId technicianId) 
        => await Context.Set<ServiceCatalog>()
            .Include(c => c.Recipes)
            .ThenInclude(r => r.ComponentRequirements)
            .FirstOrDefaultAsync(c => c.TechnicianId == technicianId);

    public async Task<ServiceCatalog?> FindByCatalogIdAsync(CatalogId catalogId)
        => await Context.Set<ServiceCatalog>()
            .AsNoTracking()
            .Include(c => c.Recipes.Where(r => r.IsActive))
            .FirstOrDefaultAsync(c => c.CatalogId == catalogId);

    public Task<bool> ExistsByTechnicianIdAsync(TechnicianId technicianId)
    {
        return Context.Set<ServiceCatalog>()
            .AnyAsync(c => c.TechnicianId == technicianId);
    }

    public async Task<ServiceRecipe?> FindActiveRecipeByIdAsync(RecipeId recipeId)
        => await Context.ServiceCatalogs
            .Where(c => c.Status == ECatalogStatus.Active)
            .SelectMany(c => c.Recipes)
            .FirstOrDefaultAsync(r => r.Id == recipeId && r.IsActive);

    public Task<ServiceRecipe?> FindActiveRecipeByCategoryAndTechnicianAsync(EServiceCategory serviceCategory, TechnicianId technicianId)
    => Context.ServiceCatalogs
        .Where(c => c.TechnicianId == technicianId && c.Status == ECatalogStatus.Active)
        .SelectMany(c => c.Recipes)
        .FirstOrDefaultAsync(r => r.ServiceCategory == serviceCategory && r.IsActive);

    public async Task<IEnumerable<ServiceCatalog>> FindAllAsync()
        => await Context.Set<ServiceCatalog>()
            .Include(c => c.Recipes)
            .ToListAsync();

    public async Task<Dictionary<TechnicianId, ServiceRecipe>> FindActiveRecipesByCategoryAndTechnicianIdsAsync(
        EServiceCategory serviceCategory, IEnumerable<TechnicianId> technicianIds)
    {
        var ids = technicianIds.ToList();
        var recipes = await Context.ServiceCatalogs
            .Where(c => ids.Contains(c.TechnicianId) && c.Status == ECatalogStatus.Active)
            .SelectMany(c => c.Recipes)
            .Where(r => r.ServiceCategory == serviceCategory && r.IsActive)
            .ToListAsync();

        return recipes
            .GroupBy(r => r.TechnicianId)
            .ToDictionary(g => g.Key, g => g.First());
    }

    public async Task<Dictionary<TechnicianId, ServiceCatalog>> FindCatalogsByTechnicianIdsAsync(
        IEnumerable<TechnicianId> technicianIds)
    {
        var ids = technicianIds.ToList();
        var catalogs = await Context.Set<ServiceCatalog>()
            .Include(c => c.Recipes)
            .Where(c => ids.Contains(c.TechnicianId))
            .ToListAsync();

        return catalogs.ToDictionary(c => c.TechnicianId, c => c);
    }
}
