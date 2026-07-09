using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.CommandServices;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST;

[Authorize(Roles = "Administrator,Agronomist")]
[ApiController]
[Route("api/v1/devices")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Agronomic Threshold endpoints")]
public class AgronomicThresholdController(
    ISensorReadingCommandService sensorReadingCommandService,
    IAgronomicThresholdQueryService agronomicThresholdQueryService
) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("{device-mac}/thresholds")]
    [SwaggerOperation(
        Summary = "Get the agronomic thresholds of an IoT device",
        Description = "Returns the agronomic thresholds configured for every sensor type of the given IoT device.",
        OperationId = "GetThreshold")]
    [SwaggerResponse(StatusCodes.Status200OK, "The thresholds were found", typeof(IEnumerable<AgronomicThresholdViewResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The IoT device was not found")]
    public async Task<IActionResult> GetThreshold(
        [FromRoute(Name = "device-mac")] string deviceMac
    )
    {
        Console.WriteLine($"[INFO] [BC] [AgronomicThreshold] GetThreshold called for deviceMac: {deviceMac}");
        try
        {
            var query = AgronomicThresholdQueryFromResourceAssembly.ToQueryFromResource(deviceMac);

            var result = await agronomicThresholdQueryService.Handle(query);

            var response =
                AgronomicThresholdViewResourceFromThresholdAggregateAssembler.ToResourceFromThresholdAggregate(
                    result
                );

            Console.WriteLine($"[INFO] [BC] [AgronomicThreshold] Thresholds retrieved for deviceMac: {deviceMac}");
            return Ok(response);
        }
        catch (Exception e) when (e is KeyNotFoundException)
        {
            Console.WriteLine($"[WARN] [BC] [AgronomicThreshold] Device not found for thresholds, deviceMac: {deviceMac} - {e.Message}");
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [BC] [AgronomicThreshold] Error getting thresholds for deviceMac: {deviceMac} - {e.Message}");
            Console.Error.WriteLine($"[GetThreshold] {e.GetType().Name}: {e.Message}");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "An unexpected error occurred." }
            );
        }
    }

    [HttpPatch("{device-mac}/thresholds")]
    [SwaggerOperation(
        Summary = "Update an agronomic threshold of an IoT device",
        Description = "Partially updates the min, max and/or description of the threshold for the given sensor type. Creates the threshold if it does not exist yet.",
        OperationId = "UpdateThreshold")]
    [SwaggerResponse(StatusCodes.Status200OK, "The threshold was updated")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid payload")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The IoT device was not found")]
    public async Task<IActionResult> UpdateThreshold(
        [FromRoute(Name = "device-mac")] string deviceMac,
        [FromBody] UpdateAgronomicThresholdResource resource
    )
    {
        Console.WriteLine($"[INFO] [BC] [AgronomicThreshold] UpdateThreshold called for deviceMac: {deviceMac}");
        try
        {
            var command = UpdateAgronomicThresholdCommandFromResourceAssembly.FromResourceToCommand(
                deviceMac,
                resource
            );
            await sensorReadingCommandService.Handle(command);

            Console.WriteLine($"[INFO] [BC] [AgronomicThreshold] Threshold updated for deviceMac: {deviceMac}");
            return Ok();
        }
        catch (Exception e) when (e is ArgumentException)
        {
            Console.WriteLine($"[WARN] [BC] [AgronomicThreshold] Invalid threshold data for deviceMac: {deviceMac} - {e.Message}");
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e) when (e is KeyNotFoundException)
        {
            Console.WriteLine($"[WARN] [BC] [AgronomicThreshold] Device not found for threshold update, deviceMac: {deviceMac} - {e.Message}");
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [BC] [AgronomicThreshold] Error updating threshold for deviceMac: {deviceMac} - {e.Message}");
            Console.Error.WriteLine($"[UpdateThreshold] {e.GetType().Name}: {e.Message}");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "An unexpected error occurred." }
            );
        }
    }
}
