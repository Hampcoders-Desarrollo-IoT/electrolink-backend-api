using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Monitoring.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Monitoring.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Monitoring.Domain.Services;
using Hampcoders.Electrolink.API.Monitoring.Interfaces.REST.Resources;
using Hampcoders.Electrolink.API.Monitoring.Interfaces.REST.Transform;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Hampcoders.Electrolink.API.Monitoring.Interfaces.REST;

[ApiController]
[Route("api/v1/service-executions")]
[Produces("application/json")]
public class ServiceExecutionsController(
    IServiceExecutionCommandService commandService,
    IServiceExecutionQueryService queryService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ServiceExecutionResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ServiceExecutionResource>> CreateServiceExecution(
        [FromBody] CreateServiceExecutionResource resource)
    {
        try
        {
            var command = CreateServiceExecutionCommandFromResourceAssembler.ToCommandFromResource(resource);
            var execution = await commandService.Handle(command);
            var result = ServiceExecutionResourceFromEntityAssembler.ToResourceFromEntity(execution);

            return CreatedAtAction(nameof(GetServiceExecutionById), new { executionId = result.ExecutionId }, result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("{executionId}", Name = nameof(GetServiceExecutionById))]
    [ProducesResponseType(typeof(ServiceExecutionResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceExecutionResource>> GetServiceExecutionById(string executionId)
    {
        var execution = await queryService.Handle(new GetServiceExecutionByIdQuery(ServiceExecutionId.From(executionId)));
        if (execution is null) return NotFound(new { message = $"ServiceExecution {executionId} not found." });
        return Ok(ServiceExecutionResourceFromEntityAssembler.ToResourceFromEntity(execution));
    }

    [HttpGet("{executionId}/work-log")]
    [ProducesResponseType(typeof(WorkLogResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkLogResource>> GetWorkLog(string executionId)
    {
        var execution = await queryService.Handle(new GetServiceExecutionByIdQuery(ServiceExecutionId.From(executionId)));
        if (execution is null) return NotFound(new { message = $"ServiceExecution {executionId} not found." });
        return Ok(ServiceExecutionResourceFromEntityAssembler.ToWorkLogResourceFromEntity(execution));
    }

    [HttpGet("technicians/{technicianId}/assigned")]
    [ProducesResponseType(typeof(IEnumerable<ServiceExecutionResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ServiceExecutionResource>>> GetAssignedByTechnician(string technicianId)
    {
        var executions = await queryService.Handle(new GetAssignedServicesByTechnicianQuery(TechnicianId.From(technicianId)));
        return Ok(executions.Select(ServiceExecutionResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("homeowners/{homeownerId}/history")]
    [ProducesResponseType(typeof(IEnumerable<ServiceExecutionResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ServiceExecutionResource>>> GetHistoryByHomeowner(string homeownerId)
    {
        var executions = await queryService.Handle(new GetServiceHistoryByClientQuery(ClientIdentity.FromHomeowner(homeownerId)));
        return Ok(executions.Select(ServiceExecutionResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("technicians/{technicianId}/history")]
    [ProducesResponseType(typeof(IEnumerable<ServiceExecutionResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ServiceExecutionResource>>> GetHistoryByTechnician(string technicianId)
    {
        var executions = await queryService.Handle(new GetServiceHistoryByTechnicianQuery(TechnicianId.From(technicianId)));
        return Ok(executions.Select(ServiceExecutionResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("homeowners/{homeownerId}/active")]
    [ProducesResponseType(typeof(ServiceExecutionResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceExecutionResource>> GetActiveByHomeowner(string homeownerId)
    {
        var execution = await queryService.Handle(new GetActiveServiceByClientQuery(ClientIdentity.FromHomeowner(homeownerId)));
        if (execution is null) return NotFound(new { message = "No active service found for this homeowner." });
        return Ok(ServiceExecutionResourceFromEntityAssembler.ToResourceFromEntity(execution));
    }
}
