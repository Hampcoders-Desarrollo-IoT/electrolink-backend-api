using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Entities;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Events;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Exceptions;
using Hampcoders.Electrolink.API.Planning.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;

public class ServiceCatalog : BaseAggregateRoot
{
    public CatalogId CatalogId { get; private set; }
    public TechnicianId TechnicianId { get; private set; }
    public ECatalogStatus Status { get; private set; }

    private readonly List<ServiceRecipe> _recipes = [];
    public IReadOnlyList<ServiceRecipe> Recipes => _recipes.AsReadOnly();

    private ServiceCatalog() { }

    // ── Factory ───────────────────────────────────────────

    public static ServiceCatalog Create(TechnicianId technicianId)
    {
        var catalog = new ServiceCatalog
        {
            CatalogId = CatalogId.NewId(),
            TechnicianId = technicianId,
            Status = ECatalogStatus.Empty,
        };
        catalog.RaiseDomainEvent(new ServiceCatalogCreatedEvent(
            catalog.CatalogId, technicianId, DateTime.UtcNow));
        return catalog;
    }

    // ── Commands ──────────────────────────────────────────

    public void AddRecipe(
        string serviceName,
        string serviceDescription,
        EServiceCategory serviceCategory,
        bool requiresIoTCertification,
        IReadOnlyList<ComponentRequirementItem> componentRequirements,
        EstimatedDuration estimatedDuration,
        ServicePricing pricing,
        IReadOnlyList<string> prerequisites,
        IReadOnlyList<string> deliverables,
        WarrantyPeriod warrantyPeriod,
        IComponentTypeValidator componentTypeValidator)
    {
        EnsureNameIsUnique(serviceName);
        if (componentRequirements.Count > 0)
            componentTypeValidator.ValidateAll(componentRequirements);

        var recipe = ServiceRecipe.Create(
            CatalogId, TechnicianId, serviceName, serviceDescription,
            serviceCategory, requiresIoTCertification, componentRequirements, estimatedDuration,
            pricing, prerequisites, deliverables, warrantyPeriod);

        _recipes.Add(recipe);

        if (Status == ECatalogStatus.Empty)
        {
            Status = ECatalogStatus.Active;
            RaiseDomainEvent(new ServiceCatalogStatusChangedEvent(
                CatalogId, ECatalogStatus.Empty, ECatalogStatus.Active, DateTime.UtcNow));
        }

        RaiseDomainEvent(new ServiceRecipeCreatedEvent(recipe, TechnicianId, DateTime.UtcNow));
    }

    public void UpdateRecipe(
        RecipeId recipeId,
        string? serviceName,
        string? serviceDescription,
        IReadOnlyList<ComponentRequirementItem>? componentRequirements,
        EstimatedDuration? estimatedDuration,
        ServicePricing? pricing,
        IReadOnlyList<string>? prerequisites,
        IReadOnlyList<string>? deliverables,
        WarrantyPeriod? warrantyPeriod,
        int activeServicesCount,
        IComponentTypeValidator componentTypeValidator)
    {
        var recipe = FindRecipeOrFail(recipeId);

        recipe.Update(serviceName, serviceDescription, componentRequirements,
            estimatedDuration, pricing, prerequisites, deliverables,
            warrantyPeriod, activeServicesCount, componentTypeValidator);

        RaiseDomainEvent(new ServiceRecipeUpdatedEvent(recipe, TechnicianId, DateTime.UtcNow));
    }

    public void DeactivateRecipe(RecipeId recipeId, DeactivationReason reason, int inProgressCount)
    {
        var recipe = FindRecipeOrFail(recipeId);
        recipe.Deactivate(reason, inProgressCount);

        if (_recipes.All(r => !r.IsActive))
        {
            Status = ECatalogStatus.Inactive;
            RaiseDomainEvent(new ServiceCatalogStatusChangedEvent(
                CatalogId, ECatalogStatus.Active, ECatalogStatus.Inactive, DateTime.UtcNow));
        }

        RaiseDomainEvent(new ServiceRecipeDeactivatedEvent(recipe, TechnicianId, reason, DateTime.UtcNow));
    }

    public void ReactivateRecipe(RecipeId recipeId)
    {
        var recipe = FindRecipeOrFail(recipeId);
        recipe.Reactivate();

        if (Status == ECatalogStatus.Inactive)
        {
            Status = ECatalogStatus.Active;
            RaiseDomainEvent(new ServiceCatalogStatusChangedEvent(
                CatalogId, ECatalogStatus.Inactive, ECatalogStatus.Active, DateTime.UtcNow));
        }

        RaiseDomainEvent(new ServiceRecipeReactivatedEvent(recipe, TechnicianId, DateTime.UtcNow));
    }

    // ── Invariants ────────────────────────────────────────

    private void EnsureNameIsUnique(string name)
    {
        if (_recipes.Any(r => r.ServiceName == name && r.IsActive))
            throw new DuplicateRecipeNameException(name);
    }

    private ServiceRecipe FindRecipeOrFail(RecipeId recipeId)
        => _recipes.FirstOrDefault(r => r.Id == recipeId)
           ?? throw new RecipeNotFoundException(recipeId.ToString());
}