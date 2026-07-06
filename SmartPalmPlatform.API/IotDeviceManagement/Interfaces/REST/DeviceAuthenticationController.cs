using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.CommandServices;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST
{
    [Route("api/v1/edges")]
    [ApiController]
    public class DeviceAuthenticationController(
        IDeviceStatusCommandService deviceStatusCommandService
    ) : ControllerBase
    {
        [HttpPost("")]
        public async Task<IActionResult> RegisterEdgeDevice(
            [FromBody] EdgeDeviceRegistrationResource resource
        )
        {
            try
            {
                var command = RegisterEdgeDeviceCommandFromResourceAssembler.ToCommandFromResource(
                    resource
                );

                await deviceStatusCommandService.Handle(command);

                return Created($"/api/v1/edges/{resource.edgeMac}", null);
            }
            catch (Exception e) when (e is UnauthorizedAccessException)
            {
                return Unauthorized(new { message = e.Message });
            }
            catch (Exception e) when (e is InvalidOperationException)
            {
                return Conflict(new { message = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = e.Message }
                );
            }
        }

        [HttpPost("{edgeMac}/iot-devices")]
        public async Task<IActionResult> RegisterIotDevice(
            [FromRoute] string edgeMac,
            [FromBody] IotDeviceRegistrationResource resource
        )
        {
            try
            {
                var command = RegisterIotDeviceCommandFromResourceAssembler.ToCommandFromResource(
                    edgeMac,
                    resource
                );

                await deviceStatusCommandService.Handle(command);

                return Created($"/api/v1/edges/{edgeMac}/iot-devices/{resource.iotMac}", null);
            }
            catch (Exception e) when (e is UnauthorizedAccessException)
            {
                return Unauthorized(new { message = e.Message });
            }
            catch (Exception e) when (e is InvalidOperationException)
            {
                return Conflict(new { message = e.Message });
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
}
