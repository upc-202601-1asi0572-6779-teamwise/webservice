using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.CommandServices;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST;

[ApiController]
[Route("api/v1/device")]
public class AgronomicThresholdController(
    ISensorReadingCommandService sensorReadingCommandService,
    IAgronomicThresholdQueryService agronomicThresholdQueryService
) : ControllerBase
{
    [HttpGet("edge/{edgeMac}/iot/{iotMac}/threshold")]
    public async Task<IActionResult> Get([FromRoute] string edgeMac, [FromRoute] string iotMac)
    {
        try
        {
            var query = AgronomicThresholdQueryFromResourceAssembly.ToQueryFromResource(
                edgeMac,
                iotMac
            );

            var result = await agronomicThresholdQueryService.Handle(query);

            var response =
                AgronomicThresholdViewResourceFromThresholdAggregateAssembler.ToResourceFromThresholdAggregate(
                    result
                );

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

    [HttpPut("edge/{edgeMac}/iot/{iotMac}/threshold")]
    public async Task<IActionResult> Post(
        [FromRoute] string edgeMac,
        [FromRoute] string iotMac,
        [FromBody] UpdateAgronomicThresholdResource resource
    )
    {
        try
        {
            var command = UpdateAgronomicThresholdCommandFromResourceAssembly.FromResourceToCommand(
                edgeMac,
                iotMac,
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
}
