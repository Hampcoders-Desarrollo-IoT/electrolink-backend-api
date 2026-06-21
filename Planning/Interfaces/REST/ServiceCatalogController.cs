using Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Exceptions;
using Hampcoders.Electrolink.API.Planning.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Planning.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Planning.Domain.Services;
using Hampcoders.Electrolink.API.Planning.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Planning.Interfaces.REST.Transform;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Interfaces.REST;
using Microsoft.AspNetCore.Mvc;

namespace Hampcoders.Electrolink.API.Planning.Interfaces.REST;

[ApiController]
[Route("api/v1/technicians/{technicianId}/catalog")]
[Produces("application/json")]
public class ServiceCatalogController(
    IServiceCatalogCommandService commandService,
    IServiceDesignQueryService queryService)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateCatalog([FromRoute] string technicianId)
    {
        try
        {
            var command = new CreateServiceCatalogCommand(TechnicianId.From(technicianId), ProfileId.From(User.GetProfileId()));
            await commandService.Handle(command);
            
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (CatalogAlreadyExistsException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(ServiceCatalogResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceCatalogResource>> GetCatalog(
        [FromRoute] string technicianId)
    {
        var catalog = await queryService.Handle(new GetServiceCatalogQuery(TechnicianId.From(technicianId)));
        if (catalog is null) return NotFound(new { message = "Catalog not found" });
        return Ok(ServiceCatalogResourceFromEntityAssembler.ToResource(catalog));
    }

    [HttpPost("recipes")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateRecipe(
        [FromBody]  CreateServiceRecipeResource resource)
    {
        try
        {
            var technicianId = User.GetTechnicianId();
            var catalog = await queryService.Handle(new GetServiceCatalogQuery(TechnicianId.From(technicianId)));
            if (catalog is null) return NotFound(new { message = "Catalog not found" });

            var command = CreateServiceRecipeCommandFromResourceAssembler
                .ToCommand(resource, catalog.CatalogId.Value, technicianId);

            var recipe = await commandService.Handle(command);
            if (recipe is null) return BadRequest();
            return CreatedAtAction(
                nameof(GetRecipeDetails),
                new { technicianId, recipeId = recipe.Id.Value },
                new { recipeId = recipe.Id.Value });
        }
        catch (DuplicateRecipeNameException ex) { return Conflict(new { message = ex.Message }); }
        catch (InvalidPricingException ex)      { return BadRequest(new { message = ex.Message }); }
    }

    [HttpGet("recipes/{recipeId}")]
    [ProducesResponseType(typeof(ServiceRecipeDetailResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceRecipeDetailResource>> GetRecipeDetails(
        [FromRoute] string technicianId,
        [FromRoute] string recipeId)
    {
        var catalog = await queryService.Handle(new GetServiceCatalogQuery(TechnicianId.From(technicianId)));
        if (catalog is null) return NotFound(new { message = "Catalog not found" });

        var recipe = catalog.Recipes.FirstOrDefault(r => r.Id.Value == recipeId);
        if (recipe is null) return NotFound(new { message = "Recipe not found" });

        return Ok(ServiceRecipeDetailResourceFromEntityAssembler.ToResource(recipe));
    }

    [HttpPatch("recipes/{recipeId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRecipe(
        [FromRoute] string technicianId,
        [FromRoute] string recipeId,
        [FromBody]  UpdateServiceRecipeResource resource)
    {
        try
        {
            var catalog = await queryService.Handle(new GetServiceCatalogQuery(TechnicianId.From(technicianId)));
            if (catalog is null) return NotFound(new { message = "Catalog not found" });

            var command = UpdateServiceRecipeCommandFromResourceAssembler.ToCommandFromResource(
                resource, 
                recipeId, 
                catalog.CatalogId.Value, 
                technicianId);
            
            await commandService.Handle(command);
            return NoContent();
        }
        catch (CannotChangeComponentsWithActiveServicesException ex) { return BadRequest(new { message = ex.Message }); }
        catch (PriceIncreaseTooLargeException ex) { return BadRequest(new { message = ex.Message }); }
        catch (RecipeNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }

    [HttpDelete("recipes/{recipeId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeactivateRecipe(
        [FromRoute] string technicianId,
        [FromRoute] string recipeId,
        [FromBody]  DeactivateServiceRecipeResource resource)
    {
        try
        {
            var catalog = await queryService.Handle(new GetServiceCatalogQuery(TechnicianId.From(technicianId)));
            if (catalog is null) return NotFound(new { message = "Catalog not found" });

            await commandService.Handle(new DeactivateServiceRecipeCommand(
                catalog.CatalogId, 
                RecipeId.From(recipeId), 
                TechnicianId.From(technicianId), 
                resource.Reason, 
                resource.Notes));
            
            return NoContent();
        }
        catch (CannotDeactivateRecipeWithInProgressServicesException ex) { return Conflict(new { message = ex.Message }); }
        catch (RecipeNotFoundException ex)                               { return NotFound(new { message = ex.Message }); }
    }

    [HttpPost("recipes/{recipeId}/reactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReactivateRecipe(
        [FromRoute] string technicianId,
        [FromRoute] string recipeId)
    {
        try
        {
            var catalog = await queryService.Handle(new GetServiceCatalogQuery(TechnicianId.From(technicianId)));
            if (catalog is null) return NotFound(new { message = "Catalog not found" });

            await commandService.Handle(new ReactivateServiceRecipeCommand(
                catalog.CatalogId, 
                RecipeId.From(recipeId), 
                TechnicianId.From(technicianId)));
            return NoContent();
        }
        catch (RecipeNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }
}