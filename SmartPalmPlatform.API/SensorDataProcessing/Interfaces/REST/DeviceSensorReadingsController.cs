using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST;

[ApiController]
[Route("api/v1/devices")]
public class DeviceSensorReadingsController(ISensorReadingQueryService sensorReadingQueryService)
    : ControllerBase
{
    [HttpGet("{device-mac}/sensor-readings")]
    public async Task<IActionResult> GetSensorReadings(
        [FromRoute(Name = "device-mac")] string deviceMac,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int size = 100
    )
    {
        try
        {
            var resolvedFrom = from ?? DateTime.MinValue;
            var resolvedTo = to ?? DateTime.MaxValue;

            var query = new DeviceSensorReadingQuery(
                deviceMac,
                resolvedFrom,
                resolvedTo,
                page,
                size
            );

            var readings = await sensorReadingQueryService.Handle(query);
            var response = SensorReadingViewResourceFromAggregateAssembler
                .ToResourceListFromAggregateList(readings);

            return Ok(response);
        }
        catch (Exception e) when (e is KeyNotFoundException)
        {
            return NotFound(new { message = e.Message });
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
