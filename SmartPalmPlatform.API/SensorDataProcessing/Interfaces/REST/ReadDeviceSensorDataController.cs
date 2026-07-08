using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.CommandServices;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST;

[ApiController]
[Route("api/v1/edge-gateways")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Gateway Sensor Reading endpoints")]
public class ReadDeviceSensorDataController(
    ISensorReadingCommandService sensorReadingCommandService,
    ISensorReadingQueryService sensorReadingQueryService,
    ILogger<ReadDeviceSensorDataController> logger
) : ControllerBase
{
    [HttpPost("{gateway-mac}/sensor-readings")]
    [SwaggerOperation(
        Summary = "Submit sensor readings from an edge gateway",
        Description = "Ingests a batch of sensor readings sent by an edge gateway, grouped by the IoT device that produced them.",
        OperationId = "SubmitSensorReadings")]
    [SwaggerResponse(StatusCodes.Status201Created, "The sensor readings were persisted")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid payload")]
    public async Task<IActionResult> SubmitSensorReadings(
        [FromRoute(Name = "gateway-mac")] string gatewayMac,
        [FromBody] ReadDeviceSensorsDataResource resource
    )
    {
        try
        {
            var command = ReadDeviceSensorsDataCommandFromResourceAssembly.FromResourceToCommand(
                gatewayMac,
                resource
            );
            await sensorReadingCommandService.Handle(command);

            return Created($"/api/v1/edge-gateways/{gatewayMac}/sensor-readings", null);
        }
        catch (Exception e) when (e is ArgumentException)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error while submitting sensor readings.");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "An unexpected error occurred." }
            );
        }
    }

    [HttpGet("{gateway-mac}/sensor-readings")]
    [SwaggerOperation(
        Summary = "Get sensor readings of an edge gateway",
        Description = "Returns the historical sensor readings of every IoT device under the given edge gateway. Supports filtering by date range and IoT device, and pagination.",
        OperationId = "GetGatewaySensorReadings")]
    [SwaggerResponse(StatusCodes.Status200OK, "The sensor readings were found", typeof(IEnumerable<SensorReadingViewResource>))]
    public async Task<IActionResult> GetSensorReadings(
        [FromRoute(Name = "gateway-mac")] string gatewayMac,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery(Name = "device-mac")] string? deviceMac,
        [FromQuery] int page = 1,
        [FromQuery] int size = 100
    )
    {
        try
        {
            var resolvedFrom = from ?? DateTime.MinValue;
            var resolvedTo   = to   ?? DateTime.MaxValue;

            var query = new SensorReadingQuery(
                gatewayMac,
                resolvedFrom,
                resolvedTo,
                deviceMac,
                page,
                size
            );

            var readings = await sensorReadingQueryService.Handle(query);
            var response = SensorReadingViewResourceFromAggregateAssembler
                .ToResourceListFromAggregateList(readings);

            return Ok(response);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error while retrieving gateway sensor readings.");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "An unexpected error occurred." }
            );
        }
    }
}
