using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.CommandServices;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST
{
    [Route("api/v1/device")]
    [ApiController]
    public class DeviceAuthenticationController(
        IDeviceStatusCommandService deviceStatusCommandService
    ) : ControllerBase
    {
        [HttpPost("edge/{edgeMac}/zone/{monitoringZoneId}/auth/register")]
        public async Task<IActionResult> RegisterEdgeDevice(
            [FromRoute] string edgeMac,
            [FromRoute] int monitoringZoneId,
            [FromBody] EdgeDeviceRegistrationResource resource
        )
        {
            try
            {
                var command = RegisterEdgeDeviceCommandFromResourceAssembler.ToCommandFromResource(
                    edgeMac,
                    monitoringZoneId,
                    resource
                );

                await deviceStatusCommandService.Handle(command);

                return Created();
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

        [HttpPost("edge/{edgeMac}/iot/{iotMac}/auth/register")]
        public async Task<IActionResult> RegisterIotDevice(
            [FromRoute] string iotMac,
            [FromRoute] string edgeMac,
            [FromBody] IotDeviceRegistrationResource resource
        )
        {
            try
            {
                var command = RegisterIotDeviceCommandFromResourceAssembler.ToCommandFromResource(
                    edgeMac,
                    iotMac,
                    resource
                );

                await deviceStatusCommandService.Handle(command);

                return Created();
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
