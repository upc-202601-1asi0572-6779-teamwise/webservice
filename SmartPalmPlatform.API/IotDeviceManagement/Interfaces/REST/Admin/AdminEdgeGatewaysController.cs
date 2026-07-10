using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Queries;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.CommandServices;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.QueryServices;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Admin;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/v1/admin/edge-gateways")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Admin Edge Gateway management")]
public class AdminEdgeGatewaysController(
    IDeviceStatusCommandService deviceStatusCommandService,
    IDeviceStatusQueryService deviceStatusQueryService
) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Register an edge gateway",
        Description = "Registers a new edge gateway device.",
        OperationId = "AdminRegisterEdgeGateway")]
    [SwaggerResponse(StatusCodes.Status201Created, "Edge gateway registered")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Edge gateway already registered")]
    public async Task<IActionResult> RegisterEdgeGateway([FromBody] EdgeDeviceRegistrationResource resource)
    {
        try
        {
            var command = RegisterEdgeDeviceCommandFromResourceAssembler.ToCommandFromResource(resource);
            await deviceStatusCommandService.Handle(command);
            return Created($"/api/v1/admin/edge-gateways/{resource.edgeMac}", null);
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            return Conflict(new { message = e.Message });
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "List all edge gateways",
        Description = "Returns all registered edge gateways.",
        OperationId = "AdminListEdgeGateways")]
    [SwaggerResponse(StatusCodes.Status200OK, "Edge gateways found", typeof(IEnumerable<ConnectivityStatusResource>))]
    public async Task<IActionResult> ListEdgeGateways()
    {
        var devices = await deviceStatusQueryService.Handle(new GetAllEdgeGatewaysQuery());
        var response = devices.Select(
            ConnectivityStatusResourceFromEdgeDeviceAggregateAssembler.ToResourceFromEdgeDeviceAggregate);
        return Ok(response);
    }

    [HttpGet("{gatewayMac}/devices")]
    [SwaggerOperation(
        Summary = "List IoT devices of an edge gateway",
        Description = "Returns IoT devices registered under the gateway.",
        OperationId = "AdminListGatewayDevices")]
    [SwaggerResponse(StatusCodes.Status200OK, "Devices found", typeof(GatewayDevicesResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Edge gateway not found")]
    public async Task<IActionResult> ListGatewayDevices(string gatewayMac)
    {
        try
        {
            var query = EdgeRegistryQueryFromResourceAssembler.ToQueryFromResource(gatewayMac);
            var result = await deviceStatusQueryService.Handle(query);
            var response = GatewayDevicesResourceFromEdgeDeviceAggregateAssembler.ToResourceFromEdgeDeviceAggregate(
                result.Item1, result.Item2);
            return Ok(response);
        }
        catch (Exception e) when (e is KeyNotFoundException)
        {
            return NotFound(new { message = e.Message });
        }
    }

    [HttpPost("{gatewayMac}/iot-devices")]
    [SwaggerOperation(
        Summary = "Register an IoT device under an edge gateway",
        Description = "Registers a new IoT device associated with the gateway.",
        OperationId = "AdminRegisterIotDevice")]
    [SwaggerResponse(StatusCodes.Status201Created, "IoT device registered")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Edge gateway not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "IoT device already registered")]
    public async Task<IActionResult> RegisterIotDevice(
        string gatewayMac,
        [FromBody] IotDeviceRegistrationResource resource)
    {
        try
        {
            var command = RegisterIotDeviceCommandFromResourceAssembler.ToCommandFromResource(gatewayMac, resource);
            await deviceStatusCommandService.Handle(command);
            return Created($"/api/v1/admin/edge-gateways/{gatewayMac}/iot-devices/{resource.iotMac}", null);
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            return Conflict(new { message = e.Message });
        }
        catch (Exception e) when (e is KeyNotFoundException)
        {
            return NotFound(new { message = e.Message });
        }
    }
}