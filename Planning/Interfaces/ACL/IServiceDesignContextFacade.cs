using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Interfaces.ACL;

public record RecipeDetailDto(
    bool? IsActive,
    string? ServiceName,
    decimal? TotalPrice,
    int? EstimatedDurationMinutes,
    int? WarrantyMonths,
    string? ServiceCategory,
    IReadOnlyList<(string componentTypeId, int quantity)>? ComponentRequirements);

/// <summary>
/// Interfaz de Anti-Corruption Layer (ACL).
/// Define las capacidades del Planning BC que otros Bounded Contexts pueden usar.
/// Aislamiento de cambios internos del dominio.
/// </summary>
public interface IServiceDesignContextFacade
{
    Task<bool> CatalogExistsForTechnicianAsync(string technicianId);
    Task<RecipeDetailDto?> GetRecipeDetailAsync(string recipeId, string technicianId);
    Task<bool> RecipeIsActiveAsync(string recipeId, string technicianId);
    Task<string?> GetRecipeNameAsync(string recipeId, string technicianId);
    Task<decimal?> GetRecipeTotalPriceAsync(string recipeId, string technicianId);
    Task<int?> GetRecipeEstimatedDurationMinutesAsync(string recipeId, string technicianId);
    Task<int?> GetRecipeWarrantyMonthsAsync(string recipeId, string technicianId);
    Task<string?> GetRecipeServiceCategoryAsync(string recipeId, string technicianId);
    Task<IReadOnlyList<(string componentTypeId, int quantity)>> GetRecipeComponentRequirementsAsync(string recipeId, string technicianId);
    Task ReactivateServiceRequestAsync(string requestId, ClientIdentity client, string reason);
}