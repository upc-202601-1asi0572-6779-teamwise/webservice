using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.CommandServices;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST;

[ApiController]
[Route("api/v1/device")]
public class ReadDeviceSensorDataController(
    ISensorReadingCommandService sensorReadingCommandService,
    ISensorReadingQueryService sensorReadingQueryService
) : ControllerBase
{
    [HttpPost("edge/{edgeMac}/digest")]
    public async Task<IActionResult> Post(
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
        catch (Exception e)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = e.Message }
            );
        }
    }

    [HttpGet("edge/{edgeMac}/readings")]
    public async Task<IActionResult> GetReadings(
        [FromRoute] string edgeMac,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to
    )
    {
        try
        {
            var query = new SensorReadingQuery(
                edgeMac,
                from ?? DateTime.SpecifyKind(DateTime.Now.AddHours(-24), DateTimeKind.Unspecified),
                to ?? DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
            );

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
