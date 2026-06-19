using Hampcoders.Electrolink.API.Planning.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Planning.Domain.Services;
using Hampcoders.Electrolink.API.Planning.Interfaces.ACL;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Planning.Application.ACL;

/// <summary>
/// Implementación del Anti-Corruption Layer (ACL).
/// Traduce agregados del dominio a DTOs para consumo de otros BCs.
/// Protege la integridad interna del BC de cambios externos.
/// </summary>
public class ServiceDesignContextFacade(
    IServiceAssignmentCommandService assignmentCommandService,
    IServiceCatalogRepository  catalogRepository,
    IServiceRequestRepository  requestRepository,
    IServiceDesignQueryService queryService,
    IServiceRequestCommandService requestCommandService)
    : IServiceDesignContextFacade
{
    public async Task<bool> CatalogExistsForTechnicianAsync(string technicianId)
        => await catalogRepository.ExistsByTechnicianIdAsync(TechnicianId.From(technicianId));

    public async Task<RecipeDetailDto?> GetRecipeDetailAsync(string recipeId, string technicianId)
    {
        var detail = await queryService.Handle(
            new GetServiceRecipeDetailsQuery(RecipeId.From(recipeId), TechnicianId.From(technicianId)));
        if (detail is null) return null;

        return new RecipeDetailDto(
            detail.IsActive,
            detail.ServiceName,
            detail.Pricing.TotalPrice.Amount,
            detail.EstimatedDuration.TotalMinutes,
            detail.WarrantyPeriod.Months,
            detail.ServiceCategory.ToString(),
            detail.ComponentRequirements.Select(c => (c.ComponentTypeId, c.Quantity)).ToList());
    }

    public async Task<bool> RecipeIsActiveAsync(string recipeId, string technicianId)
    {
        var detail = await GetRecipeDetailAsync(recipeId, technicianId);
        return detail?.IsActive ?? false;
    }

    public async Task<string?> GetRecipeNameAsync(string recipeId, string technicianId)
    {
        var detail = await GetRecipeDetailAsync(recipeId, technicianId);
        return detail?.ServiceName;
    }

    public async Task<decimal?> GetRecipeTotalPriceAsync(string recipeId, string technicianId)
    {
        var detail = await GetRecipeDetailAsync(recipeId, technicianId);
        return detail?.TotalPrice;
    }

    public async Task<int?> GetRecipeEstimatedDurationMinutesAsync(string recipeId, string technicianId)
    {
        var detail = await GetRecipeDetailAsync(recipeId, technicianId);
        return detail?.EstimatedDurationMinutes;
    }

    public async Task<int?> GetRecipeWarrantyMonthsAsync(string recipeId, string technicianId)
    {
        var detail = await GetRecipeDetailAsync(recipeId, technicianId);
        return detail?.WarrantyMonths;
    }

    public async Task<string?> GetRecipeServiceCategoryAsync(string recipeId, string technicianId)
    {
        var detail = await GetRecipeDetailAsync(recipeId, technicianId);
        return detail?.ServiceCategory;
    }

    public async Task<IReadOnlyList<(string componentTypeId, int quantity)>>
        GetRecipeComponentRequirementsAsync(string recipeId, string technicianId)
    {
        var detail = await GetRecipeDetailAsync(recipeId, technicianId);
        return detail?.ComponentRequirements ?? [];
    }

    public async Task ReactivateServiceRequestAsync(string requestId, ClientIdentity client, string reason)
    {
        await requestCommandService.Handle(new ReactivateServiceRequestCommand(RequestId.From(requestId), client, reason));
    }

    public async Task<bool> ReactivateServiceRequestForReassignmentAsync(string requestId)
    {
        try
        {
            var request = await requestRepository.FindByIdAsync(RequestId.From(requestId));
            if (request is null) return false;

            // Re-encola la solicitud para un nuevo matching
            await assignmentCommandService.Handle(
                new ExecuteMatchingAlgorithmCommand(RequestId.From(requestId)));
            return true;
        }
        catch
        {
            return false;
        }
    }
    
}


