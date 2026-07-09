using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Domain.Model.Enums;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Queries;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.CommandServices;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.QueryServices;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST
{
    [Authorize]
    [Route("api/v1/edge-gateways")]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [SwaggerTag("Available Edge Gateway Status endpoints")]
    public class DeviceStatusController(
        IDeviceStatusCommandService deviceStatusCommandService,
        IDeviceStatusQueryService deviceStatusQueryService
    ) : ControllerBase
    {
        // null = Administrator (sin restricción, ve todos los gateways).
        // Con valor = solo los gateways asignados a ese usuario.
        // Fail-closed: si por algún motivo no hay usuario en el contexto (no debería
        // ocurrir con [Authorize] activo), se filtra por un id inexistente (0) en vez
        // de tratarlo como "sin restricción".
        private int? GetOwnerFilter()
        {
            var user = HttpContext.Items["User"] as User;
            if (user is null) return 0;
            return user.Role == UserRole.Administrator ? null : user.Id;
        }

        [HttpGet("")]
        [SwaggerOperation(
            Summary = "Get all edge gateways",
            Description = "Returns every edge gateway registered in the platform, along with its connectivity status.",
            OperationId = "GetAllGateways")]
        [SwaggerResponse(StatusCodes.Status200OK, "The edge gateways were found", typeof(IEnumerable<ConnectivityStatusResource>))]
        public async Task<IActionResult> GetAllGateways()
        {
            Console.WriteLine($"[INFO] [BC] [DeviceStatus] GetAllGateways called");
            try
            {
                var devices = await deviceStatusQueryService.Handle(new GetAllEdgeGatewaysQuery(GetOwnerFilter()));

                var response = devices.Select(
                    ConnectivityStatusResourceFromEdgeDeviceAggregateAssembler.ToResourceFromEdgeDeviceAggregate
                );

                Console.WriteLine($"[INFO] [BC] [DeviceStatus] Retrieved {response.Count()} gateways");
                return Ok(response);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] [BC] [DeviceStatus] Error getting all gateways - {e.Message}");
                Console.Error.WriteLine($"[GetAllGateways] {e.GetType().Name}: {e.Message}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred." }
                );
            }
        }

        [HttpGet("{gateway-mac}/devices")]
        [SwaggerOperation(
            Summary = "Get the IoT devices of an edge gateway",
            Description = "Returns the IoT devices currently registered under the given edge gateway.",
            OperationId = "GetDevices")]
        [SwaggerResponse(StatusCodes.Status200OK, "The devices were found", typeof(GatewayDevicesResource))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The edge gateway was not found")]
        public async Task<IActionResult> GetDevices(
            [FromRoute(Name = "gateway-mac")] string gatewayMac
        )
        {
            Console.WriteLine($"[INFO] [BC] [DeviceStatus] GetDevices called for gatewayMac: {gatewayMac}");
            try
            {
                var query = EdgeRegistryQueryFromResourceAssembler.ToQueryFromResource(gatewayMac, GetOwnerFilter());
                var result = await deviceStatusQueryService.Handle(query);

                var response =
                    GatewayDevicesResourceFromEdgeDeviceAggregateAssembler.ToResourceFromEdgeDeviceAggregate(
                        result.Item1,
                        result.Item2
                    );

                Console.WriteLine($"[INFO] [BC] [DeviceStatus] Retrieved devices for gatewayMac: {gatewayMac}");
                return Ok(response);
            }
            catch (Exception e) when (e is KeyNotFoundException)
            {
                Console.WriteLine($"[WARN] [BC] [DeviceStatus] Edge gateway not found for GetDevices, gatewayMac: {gatewayMac} - {e.Message}");
                return NotFound(new { message = e.Message });
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] [BC] [DeviceStatus] Error getting devices for gatewayMac: {gatewayMac} - {e.Message}");
                Console.Error.WriteLine($"[GetDevices] {e.GetType().Name}: {e.Message}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred." }
                );
            }
        }

        [AllowAnonymous]
        [HttpPost("{gateway-mac}/synchronizations")]
        [SwaggerOperation(
            Summary = "Synchronize an edge gateway",
            Description = "Marks the edge gateway as reconnected and submits a batch of sensor readings buffered while it was offline.",
            OperationId = "SynchronizeEdge")]
        [SwaggerResponse(StatusCodes.Status200OK, "The edge gateway was synchronized")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid payload")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The edge gateway was not found")]
        public async Task<IActionResult> SynchronizeEdge(
            [FromRoute(Name = "gateway-mac")] string gatewayMac,
            [FromBody] EdgeSynchronizationResource resource
        )
        {
            Console.WriteLine($"[INFO] [BC] [DeviceStatus] SynchronizeEdge called for gatewayMac: {gatewayMac}");
            try
            {
                var command = EdgeSynchronizationCommandFromResourceAssembler.ToCommandFromResource(
                    gatewayMac,
                    resource
                );
                await deviceStatusCommandService.Handle(command);
                Console.WriteLine($"[INFO] [BC] [DeviceStatus] Edge gateway synchronized gatewayMac: {gatewayMac}");
                return Ok();
            }
            catch (Exception e) when (e is KeyNotFoundException)
            {
                Console.WriteLine($"[WARN] [BC] [DeviceStatus] Edge gateway not found for sync, gatewayMac: {gatewayMac} - {e.Message}");
                return NotFound(new { message = e.Message });
            }
            catch (Exception e) when (e is ArgumentException)
            {
                Console.WriteLine($"[WARN] [BC] [DeviceStatus] Invalid sync data for gatewayMac: {gatewayMac} - {e.Message}");
                return BadRequest(new { message = e.Message });
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] [BC] [DeviceStatus] Error synchronizing edge gateway gatewayMac: {gatewayMac} - {e.Message}");
                Console.Error.WriteLine($"[SynchronizeEdge] {e.GetType().Name}: {e.Message}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred." }
                );
            }
        }

        [AllowAnonymous]
        [HttpGet("{gateway-mac}/connectivity")]
        [SwaggerOperation(
            Summary = "Get the connectivity status of an edge gateway",
            Description = "Returns whether the given edge gateway is currently considered connected.",
            OperationId = "GetConnectivityStatus")]
        [SwaggerResponse(StatusCodes.Status200OK, "The connectivity status was found", typeof(ConnectivityStatusResource))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The edge gateway was not found")]
        public async Task<IActionResult> GetConnectivityStatus(
            [FromRoute(Name = "gateway-mac")] string gatewayMac
        )
        {
            Console.WriteLine($"[INFO] [BC] [DeviceStatus] GetConnectivityStatus called for gatewayMac: {gatewayMac}");
            try
            {
                var query = ConnectiviyStatusQueryFromResourceAssembler.ToQueryFromResource(
                    gatewayMac
                );
                var result = await deviceStatusQueryService.Handle(query);

                var response =
                    ConnectivityStatusResourceFromEdgeDeviceAggregateAssembler.ToResourceFromEdgeDeviceAggregate(
                        result
                    );

                Console.WriteLine($"[INFO] [BC] [DeviceStatus] Connectivity status retrieved for gatewayMac: {gatewayMac}");
                return Ok(response);
            }
            catch (Exception e) when (e is KeyNotFoundException)
            {
                Console.WriteLine($"[WARN] [BC] [DeviceStatus] Edge gateway not found for connectivity, gatewayMac: {gatewayMac} - {e.Message}");
                return NotFound(new { message = e.Message });
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] [BC] [DeviceStatus] Error getting connectivity status for gatewayMac: {gatewayMac} - {e.Message}");
                Console.Error.WriteLine($"[GetConnectivityStatus] {e.GetType().Name}: {e.Message}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred." }
                );
            }
        }

        [AllowAnonymous]
        [HttpGet("{gateway-mac}/registry")]
        [SwaggerOperation(
            Summary = "Get the device registry of an edge gateway",
            Description = "Legacy view of the IoT devices registered under an edge gateway. Superseded by GET /{gateway-mac}/devices, kept for backward compatibility.",
            OperationId = "GetDeviceRegistry")]
        [SwaggerResponse(StatusCodes.Status200OK, "The registry was found", typeof(EdgeRegistryResource))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The edge gateway was not found")]
        public async Task<IActionResult> GetDeviceRegistry(
            [FromRoute(Name = "gateway-mac")] string gatewayMac
        )
        {
            Console.WriteLine($"[INFO] [BC] [DeviceStatus] GetDeviceRegistry called for gatewayMac: {gatewayMac}");
            try
            {
                var query = EdgeRegistryQueryFromResourceAssembler.ToQueryFromResource(gatewayMac);
                var result = await deviceStatusQueryService.Handle(query);

                var response =
                    EdgeRegistryResourceFromEdgeDeviceAggregateAssembler.ToResourceFromEdgeDeviceAggregate(
                        result.Item1,
                        result.Item2
                    );

                Console.WriteLine($"[INFO] [BC] [DeviceStatus] Device registry retrieved for gatewayMac: {gatewayMac}");
                return Ok(response);
            }
            catch (Exception e) when (e is KeyNotFoundException)
            {
                Console.WriteLine($"[WARN] [BC] [DeviceStatus] Edge gateway not found for registry, gatewayMac: {gatewayMac} - {e.Message}");
                return NotFound(new { message = e.Message });
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] [BC] [DeviceStatus] Error getting device registry for gatewayMac: {gatewayMac} - {e.Message}");
                Console.Error.WriteLine($"[GetDeviceRegistry] {e.GetType().Name}: {e.Message}");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred." }
                );
            }
        }
    }
}
