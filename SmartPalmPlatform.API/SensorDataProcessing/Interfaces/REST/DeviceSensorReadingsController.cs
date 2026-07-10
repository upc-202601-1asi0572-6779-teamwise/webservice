using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST;

[Authorize]
[RequireActiveSubscription]
[ApiController]
[Route("api/v1/devices")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Device Sensor Reading endpoints")]
public class DeviceSensorReadingsController(
    ISensorReadingQueryService sensorReadingQueryService
) : ControllerBase
{
    [HttpGet("{device-mac}/sensor-readings")]
    [SwaggerOperation(
        Summary = "Get sensor readings of an IoT device",
        Description = "Returns the historical sensor readings of a single IoT device, regardless of which edge gateway forwarded them. Supports filtering by date range and pagination.",
        OperationId = "GetDeviceSensorReadings")]
    [SwaggerResponse(StatusCodes.Status200OK, "The sensor readings were found", typeof(IEnumerable<SensorReadingViewResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The IoT device was not found")]
    public async Task<IActionResult> GetSensorReadings(
        [FromRoute(Name = "device-mac")] string deviceMac,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int size = 100
    )
    {
        Console.WriteLine($"[INFO] [BC] [DeviceSensorReadings] GetSensorReadings called for deviceMac: {deviceMac}, from: {from}, to: {to}, page: {page}, size: {size}");
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

            Console.WriteLine($"[INFO] [BC] [DeviceSensorReadings] Retrieved {response.Count()} sensor readings for deviceMac: {deviceMac}");
            return Ok(response);
        }
        catch (Exception e) when (e is KeyNotFoundException)
        {
            Console.WriteLine($"[WARN] [BC] [DeviceSensorReadings] Device not found for sensor readings, deviceMac: {deviceMac} - {e.Message}");
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [BC] [DeviceSensorReadings] Error getting sensor readings for deviceMac: {deviceMac} - {e.Message}");
            Console.Error.WriteLine($"[GetDeviceSensorReadings] {e.GetType().Name}: {e.Message}");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "An unexpected error occurred." }
            );
        }
    }
}
