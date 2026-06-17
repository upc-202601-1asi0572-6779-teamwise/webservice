using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST
{
    [Route("api/v1/device/status")]
    [ApiController]
    public class DeviceStatusController : ControllerBase
    {
        private readonly IDeviceStatusCommandService _deviceStatusCommandService;
        private readonly IDeviceStatusQueryService _deviceStatusQueryService;

        public DeviceStatusController(
            IDeviceStatusCommandService deviceStatusCommandService,
            IDeviceStatusQueryService deviceStatusQueryService
        )
        {
            _deviceStatusCommandService = deviceStatusCommandService;
            _deviceStatusQueryService = deviceStatusQueryService;
        }

        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivateDevice([FromBody] SerialResource resource)
        {
            try
            {
                var command = DeactivateDeviceCommandFromResourceAssembler.ToCommandFromResource(
                    resource.serialNumber
                );
                await _deviceStatusCommandService.Handle(command);

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

        [HttpPost("activate")]
        public async Task<IActionResult> ActivateDevice([FromBody] SerialResource resource)
        {
            try
            {
                var command = ActivateDeviceCommmandFromResourceAssembler.ToCommandFromResource(
                    resource.serialNumber
                );
                await _deviceStatusCommandService.Handle(command);

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

        [HttpGet("activation")]
        public async Task<IActionResult> GetActivationStatus([FromQuery] SerialResource resource)
        {
            try
            {
                var query = ActivationStatusQueryFromResourceAssembler.ToQueryFromResource(
                    resource.serialNumber
                );
                var result = await _deviceStatusQueryService.Handle(query);

                var response =
                    ActivationStatusResourceFromIotDeviceAggregateAssembler.ToResourceFromIotDeviceAggregate(
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

        [HttpGet("connectivity")]
        public async Task<IActionResult> GetConnectivityStatus([FromQuery] SerialResource resource)
        {
            try
            {
                var query = ConnectiviyStatusQueryFromResourceAssembler.ToQueryFromResource(
                    resource.serialNumber
                );
                var result = await _deviceStatusQueryService.Handle(query);

                var response =
                    ConnectivityStatusResourceFromIotDeviceAggregateAssembler.ToResourceFromIotDeviceAggregate(
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
    }
}
