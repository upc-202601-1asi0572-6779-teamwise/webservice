using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.CommandServices;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST;

[ApiController]
[Route("api/v1/device")]
public class ReadDeviceSensorDataController : ControllerBase
{
    private readonly ISensorReadingCommandService _sensorReadingCommandService;

    public ReadDeviceSensorDataController(ISensorReadingCommandService sensorReadingCommandService)
    {
        _sensorReadingCommandService = sensorReadingCommandService;
    }

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
            await _sensorReadingCommandService.Handle(command);

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
