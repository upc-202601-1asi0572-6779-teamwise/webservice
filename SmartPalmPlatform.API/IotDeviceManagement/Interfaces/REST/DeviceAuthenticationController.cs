using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST
{
    [Route("api/v1/device/auth")]
    [ApiController]
    public class DeviceAuthenticationController : ControllerBase
    {
        private readonly IDeviceStatusCommandService _deviceStatusCommandService;

        public DeviceAuthenticationController(
            IDeviceStatusCommandService deviceStatusCommandService
        )
        {
            _deviceStatusCommandService = deviceStatusCommandService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterDevice(
            [FromBody] DeviceRegistrationResource resource
        )
        {
            try
            {
                var command = RegisterDeviceCommandFromResourceAssembler.ToCommandFromResource(
                    resource
                );

                await _deviceStatusCommandService.Handle(command);

                return Created();
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
