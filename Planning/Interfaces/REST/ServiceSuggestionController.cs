using Hampcoders.Electrolink.API.Planning.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Planning.Domain.Repositories;
using Hampcoders.Electrolink.API.Planning.Domain.Services;
using Hampcoders.Electrolink.API.Planning.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Planning.Interfaces.REST.Transform;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using Hampcoders.Electrolink.API.Shared.Interfaces.REST;
using Microsoft.AspNetCore.Mvc;

namespace Hampcoders.Electrolink.API.Planning.Interfaces.REST;

[ApiController]
[Route("api/v1/suggestions")]
[Produces("application/json")]
public class ServiceSuggestionController(
    IServiceSuggestionQueryService queryService,
    IServiceSuggestionCommandService commandService,
    IServiceSuggestionRepository suggestionRepository,
    IUnitOfWork unitOfWork)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ServiceSuggestionResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ServiceSuggestionResource>>> GetActiveSuggestions()
    {
        var clientId = User.GetClientIdentity().ClientId;
        var suggestions = await queryService.GetActiveByClientIdAsync(clientId);
        return Ok(suggestions.Select(ServiceSuggestionResourceFromEntityAssembler.ToResource));
    }

    [HttpPatch("{suggestionId}/viewed")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsViewed([FromRoute] string suggestionId)
    {
        var suggestion = await suggestionRepository.FindBySuggestionIdAsync(suggestionId);
        if (suggestion is null)
            return NotFound(new { message = $"Suggestion {suggestionId} not found." });

        var clientId = User.GetClientIdentity().ClientId;
        if (suggestion.ClientId != clientId)
            return Forbid();

        suggestion.MarkAsViewed();
        await unitOfWork.CompleteAsync();

        return NoContent();
    }

    [HttpPost("{suggestionId}/accept")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AcceptSuggestion([FromRoute] string suggestionId)
    {
        try
        {
            var client = User.GetClientIdentity();
            var requestId = await commandService.Handle(
                new AcceptServiceSuggestionCommand(suggestionId, client));
            return Ok(new { requestId });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpPost("{suggestionId}/dismiss")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DismissSuggestion([FromRoute] string suggestionId)
    {
        try
        {
            var client = User.GetClientIdentity();
            await commandService.Handle(
                new DismissServiceSuggestionCommand(suggestionId, client));
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }
}
