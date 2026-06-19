using Hampcoders.Electrolink.API.Assets.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Assets.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Assets.Domain.Services;
using Hampcoders.Electrolink.API.Assets.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Assets.Interfaces.REST.Transform;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Shared.Interfaces.REST;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Hampcoders.Electrolink.API.Assets.Interfaces.REST;

[ApiController]
[Route("api/v1/homeowners/{homeownerId}/[controller]")]
[Produces("application/json")]
[SwaggerTag("Properties Controller Endpoints")]
public class PropertiesController(
    IPropertyCommandService commandService,
    IPropertyQueryService   queryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PropertyResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PropertyResource>>> GetAll(
        [FromRoute] string homeownerId,
        [FromQuery] string? city, [FromQuery] string? district,
        [FromQuery] string? region, [FromQuery] string? street,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var results = await queryService.Handle(
            new GetAllPropertiesByOwnerIdQuery(ClientIdentity.FromHomeowner(homeownerId), city, street, page, pageSize));
        return Ok(results.Select(PropertyResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Crea una propiedad para un homeowner</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PropertyResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PropertyResource>> CreateProperty(
        [FromRoute] string homeownerId, [FromBody] CreatePropertyResource resource)
    {
        try
        {
            var command  = CreatePropertyCommandFromResourceAssembler.ToCommandFromResource(resource, homeownerId);
            var property = await commandService.Handle(command);
            if (property is null) return BadRequest();
            var response = PropertyResourceFromEntityAssembler.ToResourceFromEntity(property);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Retorna una propiedad por ID</summary>
    [HttpGet("{propertyId}", Name = nameof(GetPropertyById))]
    [ProducesResponseType(typeof(PropertyResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropertyResource>> GetPropertyById([FromRoute] string homeownerId, string propertyId)
    {
        var property = await queryService.Handle(new GetPropertyByIdQuery(PropertyId.From(propertyId), ClientIdentity.FromHomeowner(homeownerId)));
        if (property is null) return NotFound(new { message = $"Property {propertyId} not found." });
        return Ok(PropertyResourceFromEntityAssembler.ToResourceFromEntity(property));
    }

    /// <summary>Actualiza solo la dirección textual</summary>
    [HttpPatch("{propertyId}/address")]
    [ProducesResponseType(typeof(PropertyResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropertyResource>> UpdateAddress(
        [FromRoute] string homeownerId,
        [FromRoute] string propertyId, [FromBody] UpdateAddressResource resource)
    {
        try
        {
            var command = UpdatePropertyAddressCommandFromResourceAssembler.ToCommandFromResource(resource, propertyId);
            var property = await commandService.Handle(command);
            if (property is null) return NotFound();
            return Ok(PropertyResourceFromEntityAssembler.ToResourceFromEntity(property));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Actualiza la geolocalización de la propiedad</summary>
    [HttpPatch("{propertyId}/geolocation")]
    [ProducesResponseType(typeof(PropertyResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PropertyResource>> UpdateGeolocation(
        [FromRoute] string homeownerId,
        [FromRoute] string propertyId, [FromBody] UpdateGeolocationResource resource)
    {
        try
        {
            var command = UpdatePropertyGeolocationCommandFromResourceAssembler.ToCommandFromResource(resource, propertyId);
            var property = await commandService.Handle(command);
            if (property is null) return NotFound();
            return Ok(PropertyResourceFromEntityAssembler.ToResourceFromEntity(property));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Activa una propiedad</summary>
    [HttpPatch("{propertyId}/activate")]
    [ProducesResponseType(typeof(PropertyResource), StatusCodes.Status200OK)]
    public async Task<ActionResult<PropertyResource>> Activate([FromRoute] string homeownerId, [FromRoute] string propertyId)
    {
        var property = await commandService.Handle(new ActivatePropertyCommand(PropertyId.From(propertyId)));
        return property is null ? NotFound() : Ok(PropertyResourceFromEntityAssembler.ToResourceFromEntity(property));
    }

    /// <summary>Desactiva una propiedad</summary>
    [HttpPatch("{propertyId}/deactivate")]
    [ProducesResponseType(typeof(PropertyResource), StatusCodes.Status200OK)]
    public async Task<ActionResult<PropertyResource>> Deactivate([FromRoute] string homeownerId, [FromRoute] string propertyId)
    {
        var property = await commandService.Handle(new DeactivatePropertyCommand(PropertyId.From(propertyId)));
        return property is null ? NotFound() : Ok(PropertyResourceFromEntityAssembler.ToResourceFromEntity(property));
    }

    /// <summary>Archiva permanentemente una propiedad</summary>
    [HttpDelete("{propertyId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ArchiveProperty([FromRoute] string homeownerId, [FromRoute] string propertyId, [FromQuery] string reason)
    {
        try
        {
            var command = ArchivePropertyCommandFromResourceAssembler.ToCommandFromResource(propertyId, reason);
            var property = await commandService.Handle(command);
            return property is null ? NotFound() : NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Obtiene una URL firmada para subir una foto directamente a Cloudinary</summary>
    [HttpGet("{propertyId}/photos/upload-url")]
    [ProducesResponseType(typeof(PropertyPhotoUploadUrlResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropertyPhotoUploadUrlResource>> GetPhotoUploadUrl(
        [FromRoute] string homeownerId, [FromRoute] string propertyId)
    {
        try
        {
            var command = GetPropertyPhotoUploadUrlCommandFromResourceAssembler.ToCommand(homeownerId, propertyId);
            var signedData = await commandService.Handle(command);

            return Ok(new PropertyPhotoUploadUrlResource(
                signedData.Url,
                signedData.Signature,
                signedData.Timestamp,
                signedData.ApiKey));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Registra una foto después de que el cliente la subió directamente a Cloudinary</summary>
    [HttpPost("{propertyId}/photos")]
    [ProducesResponseType(typeof(PropertyResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PropertyResource>> RegisterPhoto(
        [FromRoute] string homeownerId,
        [FromRoute] string propertyId,
        [FromBody] RegisterPropertyPhotoResource resource)
    {
        try
        {
            var command = RegisterPropertyPhotoCommandFromResourceAssembler.ToCommand(homeownerId, propertyId, resource);
            var property = await commandService.Handle(command);

            if (property is null) return NotFound();
            return Ok(PropertyResourceFromEntityAssembler.ToResourceFromEntity(property));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Establece una foto existente como la foto principal de la propiedad</summary>
    [HttpPost("{propertyId}/main-photo")]
    [ProducesResponseType(typeof(PropertyResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PropertyResource>> SetMainPhoto(
        [FromRoute] string homeownerId,
        [FromRoute] string propertyId,
        [FromBody] SetPropertyMainPhotoResource resource)
    {
        try
        {
            var command = SetPropertyMainPhotoCommandFromResourceAssembler.ToCommand(homeownerId, propertyId, resource);
            var property = await commandService.Handle(command);

            if (property is null) return NotFound();
            return Ok(PropertyResourceFromEntityAssembler.ToResourceFromEntity(property));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}