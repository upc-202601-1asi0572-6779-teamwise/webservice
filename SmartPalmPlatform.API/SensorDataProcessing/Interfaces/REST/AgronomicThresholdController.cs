using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.CommandServices;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST;

[ApiController]
[Route("api/v1/devices")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Agronomic Threshold endpoints")]
public class AgronomicThresholdController(
    ISensorReadingCommandService sensorReadingCommandService,
    IAgronomicThresholdQueryService agronomicThresholdQueryService,
    ILogger<AgronomicThresholdController> logger
) : ControllerBase
{
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
        try
        {
            var query = AgronomicThresholdQueryFromResourceAssembly.ToQueryFromResource(deviceMac);

            var result = await agronomicThresholdQueryService.Handle(query);

            var response =
                AgronomicThresholdViewResourceFromThresholdAggregateAssembler.ToResourceFromThresholdAggregate(
                    result
                );

            return Ok(response);
        }
        catch (Exception e) when (e is KeyNotFoundException)
        {
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error while retrieving thresholds for device.");
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
        try
        {
            var command = UpdateAgronomicThresholdCommandFromResourceAssembly.FromResourceToCommand(
                deviceMac,
                resource
            );
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
        catch (Exception e)
        {
            logger.LogError(e, "Unexpected error while updating threshold for device.");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "An unexpected error occurred." }
            );
        }
    }
}
