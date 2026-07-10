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
[Route("api/v1/sectors")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Sector health and sensor readings endpoints")]
public class SectorDataController(
    ISectorHealthQueryService sectorHealthQueryService,
    ISensorReadingQueryService sensorReadingQueryService
) : ControllerBase
{
    [HttpGet("{sectorId:int}/health")]
    [SwaggerOperation(
        Summary = "Get sector health status",
        Description = "Returns the health status of a sector based on threshold evaluations.",
        OperationId = "GetSectorHealth")]
    [SwaggerResponse(StatusCodes.Status200OK, "Health status found")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Sector not found")]
    public async Task<IActionResult> GetSectorHealth(int sectorId)
    {
        var query = new GetSectorHealthQuery(sectorId);
        var result = await sectorHealthQueryService.Handle(query);

        if (result is null)
            return NotFound(new { message = "Sector not found or has no IoT device assigned." });

        return Ok(result);
    }

    [HttpGet("{sectorId:int}/sensor-readings")]
    [SwaggerOperation(
        Summary = "Get sensor readings for a sector",
        Description = "Returns all sensor readings for a sector's IoT device within the given date range.",
        OperationId = "GetSectorSensorReadings")]
    [SwaggerResponse(StatusCodes.Status200OK, "Sensor readings found", typeof(IEnumerable<SensorReadingViewResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Sector not found or has no IoT device")]
    public async Task<IActionResult> GetSectorSensorReadings(
        int sectorId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to
    )
    {
        try
        {
            var resolvedFrom = from ?? DateTime.MinValue;
            var resolvedTo = to ?? DateTime.MaxValue;

            var query = new SectorSensorDataQuery(sectorId, resolvedFrom, resolvedTo);
            var readings = await sensorReadingQueryService.Handle(query);
            var response = SensorReadingViewResourceFromAggregateAssembler
                .ToResourceListFromAggregateList(readings);

            return Ok(response);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
    }
}
