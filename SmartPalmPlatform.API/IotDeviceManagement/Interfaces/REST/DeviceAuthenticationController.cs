using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.CommandServices;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST
{
    [Route("api/v1/edge-gateways")]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    [SwaggerTag("Available Edge Gateway Registration endpoints")]
    public class DeviceAuthenticationController(
        IDeviceStatusCommandService deviceStatusCommandService,
        ILogger<DeviceAuthenticationController> logger
    ) : ControllerBase
    {
        [HttpPost("")]
        [SwaggerOperation(
            Summary = "Register an edge gateway",
            Description = "Registers a new edge gateway device identified by its MAC address, assigns it to a monitoring zone and a client user. Administrator only.",
            OperationId = "RegisterEdgeDevice")]
        [SwaggerResponse(StatusCodes.Status201Created, "The edge gateway was registered")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "No valid token provided")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "The caller is not an Administrator")]
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
                logger.LogError(e, "Unexpected error while registering edge device.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred." }
                );
            }
        }

        [HttpPost("{gateway-mac}/iot-devices")]
        [SwaggerOperation(
            Summary = "Register an IoT device under an edge gateway",
            Description = "Registers a new IoT device identified by its MAC address, associating it with the edge gateway given in the route. Administrator only.",
            OperationId = "RegisterIotDevice")]
        [SwaggerResponse(StatusCodes.Status201Created, "The IoT device was registered")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "No valid token provided")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "The caller is not an Administrator")]
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
                logger.LogError(e, "Unexpected error while registering IoT device.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred." }
                );
            }
        }
    }
}
