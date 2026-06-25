using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.CommandServices;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST;

[ApiController]
[Route("api/v1/edges")]
public class ReadDeviceSensorDataController(
    ISensorReadingCommandService sensorReadingCommandService,
    ISensorReadingQueryService sensorReadingQueryService
) : ControllerBase
{
    [HttpPost("{edgeMac}/sensor-readings")]
    public async Task<IActionResult> SubmitSensorReadings(
        [FromRoute] string edgeMac,
        [FromBody] ReadDeviceSensorsDataResource resource
    )
    {
        try
        {
            var command = ReadDeviceSensorsDataCommandFromResourceAssembly.FromResourceToCommand(
                edgeMac,
                resource
            );
            await sensorReadingCommandService.Handle(command);

            return Ok();
        }
        catch (Exception e) when (e is ArgumentException)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = e.Message }
            );
        }
    }

    [HttpGet("{edgeMac}/sensor-readings")]
    public async Task<IActionResult> GetSensorReadings(
        [FromRoute] string edgeMac,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to
    )
    {
        try
        {
            var resolvedFrom = from ?? DateTime.MinValue;
            var resolvedTo   = to   ?? DateTime.MaxValue;

            var query = new SensorReadingQuery(edgeMac, resolvedFrom, resolvedTo);

            var readings = await sensorReadingQueryService.Handle(query);
            var response = SensorReadingViewResourceFromAggregateAssembler
                .ToResourceListFromAggregateList(readings);

            return Ok(response);
        }
        catch (Exception e)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = e.Message }
            );
        }
    }
}
