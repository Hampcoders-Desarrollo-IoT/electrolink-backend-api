using System.Net.Mime;
using Hampcoders.Electrolink.API.Assets.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Assets.Domain.Services;
using Hampcoders.Electrolink.API.Assets.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Assets.Interfaces.REST.Transform;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST;

[ApiController]
[Route("api/v1/homeowners/{homeownerId}/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Property Portfolio Endpoints")]
public class PropertyPortfoliosController(
    IPropertyPortfolioCommandService commandService,
    IPropertyPortfolioQueryService queryService) : ControllerBase
{
    /// <summary>Retorna el portfolio del homeowner</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get portfolio by owner ID", OperationId = "GetPortfolioByOwnerId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Portfolio found", typeof(PropertyPortfolioResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Portfolio not found")]
    public async Task<IActionResult> GetPortfolio([FromRoute] string homeownerId)
    {
        var query = new GetPortfolioByOwnerIdQuery(ClientIdentity.FromHomeowner(homeownerId));
        var portfolio = await queryService.Handle(query);
        if (portfolio is null)
            return NotFound(new { message = $"Portfolio for owner {homeownerId} not found." });

        return Ok(PropertyPortfolioResourceFromEntityAssembler.ToResourceFromEntity(portfolio));
    }

    /// <summary>Agrega una propiedad al portfolio del homeowner</summary>
    [HttpPost("properties")]
    [SwaggerOperation(Summary = "Add property to portfolio", OperationId = "AddPropertyToPortfolio")]
    [SwaggerResponse(StatusCodes.Status200OK, "Property added to portfolio", typeof(PropertyPortfolioResource))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Property already in portfolio or nickname in use")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Portfolio or property not found")]
    public async Task<IActionResult> AddPropertyToPortfolio(
        [FromRoute] string homeownerId,
        [FromBody] AddPropertyToPortfolioResource resource)
    {
        try
        {
            var command = AddPropertyToPortfolioCommandFromResourceAssembler.ToCommandFromResource(resource, homeownerId);
            var portfolio = await commandService.Handle(command);
            if (portfolio is null) return BadRequest();
            return Ok(PropertyPortfolioResourceFromEntityAssembler.ToResourceFromEntity(portfolio));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    /// <summary>Elimina una propiedad del portfolio del homeowner</summary>
    [HttpDelete("properties/{propertyId}")]
    [SwaggerOperation(Summary = "Remove property from portfolio", OperationId = "RemovePropertyFromPortfolio")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Property removed from portfolio")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Portfolio or property not found")]
    public async Task<IActionResult> RemovePropertyFromPortfolio(
        [FromRoute] string homeownerId,
        [FromRoute] string propertyId,
        [FromQuery] string reason)
    {
        try
        {
            var command = RemovePropertyFromPortfolioCommandFromResourceAssembler.ToCommandFromResource(homeownerId, propertyId, reason);
            var result = await commandService.Handle(command);
            return result ? NoContent() : NotFound();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}