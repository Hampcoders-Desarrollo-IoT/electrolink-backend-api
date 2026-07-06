using Hampcoders.Electrolink.API.Processing.Domain.Model.Commands;
using Hampcoders.Electrolink.API.Processing.Domain.Services;
using Hampcoders.Electrolink.API.Processing.Infrastructure.Pipeline.Middleware.Attributes;
using Hampcoders.Electrolink.API.Processing.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Hampcoders.Electrolink.API.Processing.Interfaces.REST;

[ApiController]
[Route("api/v1/iot/edge")]
[Produces("application/json")]
[RequireEdgeApiKey]
public class EdgeIngestionController(
    IDeviceReadingStreamCommandService streamService,
    IRelayCommandService               relayService) : ControllerBase
{
    [HttpPost("readings")]
    [SwaggerOperation(Summary = "Receive a sensor reading from the Edge API", OperationId = "IngestReading")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> IngestReading([FromBody] IngestReadingResource resource)
    {
        var command = new IngestDeviceReadingCommand(
            resource.DeviceId, resource.ReadingId, resource.Timestamp,
            resource.Voltage, resource.Current, resource.PowerFactor,
            resource.Frequency, resource.Source);

        bool accepted = await streamService.Handle(command);
        return accepted ? NoContent() : Conflict(new { message = "Reading rejected." });
    }

    [HttpPost("anomalies")]
    [SwaggerOperation(Summary = "Report an edge-detected anomaly from firmware", OperationId = "ReportEdgeAnomaly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ReportEdgeAnomaly([FromBody] ReportEdgeAnomalyResource resource)
    {
        var command = new ReportEdgeAnomalyCommand(
            resource.DeviceId, resource.AnomalyType,
            resource.TriggerReadingId, resource.EdgeAlertPayloadJson);

        await streamService.Handle(command);
        return NoContent();
    }

    [HttpPost("relay/acknowledge")]
    [SwaggerOperation(Summary = "Acknowledge relay command execution from device", OperationId = "AcknowledgeRelay")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcknowledgeRelay([FromBody] AcknowledgeRelayResource resource)
    {
        try
        {
            var command = new AcknowledgeRelayExecutionCommand(
                resource.CommandId, resource.DeviceId,
                resource.ExecutedSuccessfully, resource.ExecutedAt);

            await relayService.Handle(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
    }
}
