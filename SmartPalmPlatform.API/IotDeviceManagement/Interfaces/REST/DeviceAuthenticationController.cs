using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.CommandServices;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST
{
    [Authorize(Roles = "Administrator")]
    [Route("api/v1/edge-gateways")]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [SwaggerTag("Available Edge Gateway Registration endpoints")]
    public class DeviceAuthenticationController(
        IDeviceStatusCommandService deviceStatusCommandService
    ) : ControllerBase
    {
        [HttpPost("")]
        [SwaggerOperation(
            Summary = "Register an edge gateway",
            Description = "Registers a new edge gateway device identified by its MAC address and assigns it to a monitoring zone.",
            OperationId = "RegisterEdgeDevice")]
        [SwaggerResponse(StatusCodes.Status201Created, "The edge gateway was registered")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Only administrators can register edge gateways")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "The edge gateway is already registered")]
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

                return Created($"/api/v1/edge-gateways/{resource.edgeMac}", null);
            }
            catch (Exception e) when (e is InvalidOperationException)
            {
                return Conflict(new { message = e.Message });
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"[RegisterEdgeDevice] {e.GetType().Name}: {e.Message}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred." }
                );
            }
        }

        [HttpPost("{gateway-mac}/iot-devices")]
        [SwaggerOperation(
            Summary = "Register an IoT device under an edge gateway",
            Description = "Registers a new IoT device identified by its MAC address, associating it with the edge gateway given in the route.",
            OperationId = "RegisterIotDevice")]
        [SwaggerResponse(StatusCodes.Status201Created, "The IoT device was registered")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Only administrators can register IoT devices")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The edge gateway was not found")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "The IoT device is already registered")]
        public async Task<IActionResult> RegisterIotDevice(
            [FromRoute(Name = "gateway-mac")] string gatewayMac,
            [FromBody] IotDeviceRegistrationResource resource
        )
        {
            try
            {
                var command = RegisterIotDeviceCommandFromResourceAssembler.ToCommandFromResource(
                    gatewayMac,
                    resource
                );

                await deviceStatusCommandService.Handle(command);

                return Created(
                    $"/api/v1/edge-gateways/{gatewayMac}/iot-devices/{resource.iotMac}",
                    null
                );
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
                Console.Error.WriteLine($"[RegisterIotDevice] {e.GetType().Name}: {e.Message}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred." }
                );
            }
        }
    }
}
