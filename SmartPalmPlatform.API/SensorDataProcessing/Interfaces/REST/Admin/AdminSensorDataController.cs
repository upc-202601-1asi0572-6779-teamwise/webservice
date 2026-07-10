using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.CommandServices;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Admin;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/v1/admin/devices")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Admin Device Data management")]
public class AdminSensorDataController(
    ISensorReadingCommandService sensorReadingCommandService,
    IAgronomicThresholdQueryService agronomicThresholdQueryService
) : ControllerBase
{
    [HttpGet("{deviceMac}/thresholds")]
    [SwaggerOperation(
        Summary = "Get agronomic thresholds of a device",
        Description = "Returns thresholds for every sensor type of the device.",
        OperationId = "AdminGetThresholds")]
    [SwaggerResponse(StatusCodes.Status200OK, "Thresholds found", typeof(IEnumerable<AgronomicThresholdViewResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Device not found")]
    public async Task<IActionResult> GetThresholds(string deviceMac)
    {
        try
        {
            var query = AgronomicThresholdQueryFromResourceAssembly.ToQueryFromResource(deviceMac);
            var result = await agronomicThresholdQueryService.Handle(query);
            var response = AgronomicThresholdViewResourceFromThresholdAggregateAssembler.ToResourceFromThresholdAggregate(result);
            return Ok(response);
        }
        catch (Exception e) when (e is KeyNotFoundException)
        {
            return NotFound(new { message = e.Message });
        }
    }

    [HttpPatch("{deviceMac}/thresholds")]
    [SwaggerOperation(
        Summary = "Update agronomic threshold of a device",
        Description = "Partially updates the min, max and/or description of a threshold.",
        OperationId = "AdminUpdateThreshold")]
    [SwaggerResponse(StatusCodes.Status200OK, "Threshold updated")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Device not found")]
    public async Task<IActionResult> UpdateThreshold(
        string deviceMac,
        [FromBody] UpdateAgronomicThresholdResource resource)
    {
        try
        {
            var command = UpdateAgronomicThresholdCommandFromResourceAssembly.FromResourceToCommand(deviceMac, resource);
            await sensorReadingCommandService.Handle(command);
            return Ok();
        }
        catch (Exception e) when (e is ArgumentException)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e) when (e is KeyNotFoundException)
        {
            return NotFound(new { message = e.Message });
        }
    }
}