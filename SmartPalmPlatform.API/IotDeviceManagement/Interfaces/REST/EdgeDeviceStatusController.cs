using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Queries;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.CommandServices;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.QueryServices;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST
{
    [Route("api/v1/edge-gateways")]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    [SwaggerTag("Available Edge Gateway Status endpoints")]
    public class DeviceStatusController(
        IDeviceStatusCommandService deviceStatusCommandService,
        IDeviceStatusQueryService deviceStatusQueryService,
        ILogger<DeviceStatusController> logger
    ) : ControllerBase
    {
        [HttpGet("")]
        [SwaggerOperation(
            Summary = "Get all edge gateways",
            Description = "Returns every edge gateway registered in the platform, along with its connectivity status.",
            OperationId = "GetAllGateways")]
        [SwaggerResponse(StatusCodes.Status200OK, "The edge gateways were found", typeof(IEnumerable<ConnectivityStatusResource>))]
        public async Task<IActionResult> GetAllGateways()
        {
            try
            {
                var devices = await deviceStatusQueryService.Handle(new GetAllEdgeGatewaysQuery());

                var response = devices.Select(
                    ConnectivityStatusResourceFromEdgeDeviceAggregateAssembler.ToResourceFromEdgeDeviceAggregate
                );

                return Ok(response);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unexpected error while listing edge gateways.");
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
            try
            {
                var query = EdgeRegistryQueryFromResourceAssembler.ToQueryFromResource(gatewayMac);
                var result = await deviceStatusQueryService.Handle(query);

                var response =
                    GatewayDevicesResourceFromEdgeDeviceAggregateAssembler.ToResourceFromEdgeDeviceAggregate(
                        result.Item1,
                        result.Item2
                    );

                return Ok(response);
            }
            catch (Exception e) when (e is KeyNotFoundException)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unexpected error while listing gateway devices.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred." }
                );
            }
        }

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
            try
            {
                var command = EdgeSynchronizationCommandFromResourceAssembler.ToCommandFromResource(
                    gatewayMac,
                    resource
                );
                await deviceStatusCommandService.Handle(command);
                return Ok();
            }
            catch (Exception e) when (e is KeyNotFoundException)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception e) when (e is ArgumentException)
            {
                return BadRequest(new { message = e.Message });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unexpected error while synchronizing edge gateway.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred." }
                );
            }
        }

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

                return Ok(response);
            }
            catch (Exception e) when (e is KeyNotFoundException)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unexpected error while retrieving gateway connectivity.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred." }
                );
            }
        }

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
            try
            {
                var query = EdgeRegistryQueryFromResourceAssembler.ToQueryFromResource(gatewayMac);
                var result = await deviceStatusQueryService.Handle(query);

                var response =
                    EdgeRegistryResourceFromEdgeDeviceAggregateAssembler.ToResourceFromEdgeDeviceAggregate(
                        result.Item1,
                        result.Item2
                    );

                return Ok(response);
            }
            catch (Exception e) when (e is KeyNotFoundException)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unexpected error while retrieving gateway registry.");
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred." }
                );
            }
        }
    }
}
