using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.CommandServices;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.QueryServices;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST
{
    [Route("api/v1/device")]
    [ApiController]
    public class DeviceStatusController(
        IDeviceStatusCommandService deviceStatusCommandService,
        IDeviceStatusQueryService deviceStatusQueryService
    ) : ControllerBase
    {
        [HttpPost("edge/{edgeMac}/digest/reconect")]
        public async Task<IActionResult> RestoreEdgeDevice(
            [FromRoute] string edgeMac,
            [FromBody] EdgeSynchronizationResource resource
        )
        {
            try
            {
                var command = EdgeSynchronizationCommandFromResourceAssembler.ToCommandFromResource(
                    edgeMac,
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
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = e.Message }
                );
            }
        }

        [HttpGet("edge/{edgeMac}/status/connectivity")]
        public async Task<IActionResult> GetConnectivityStatus([FromRoute] string edgeMac)
        {
            try
            {
                var query = ConnectiviyStatusQueryFromResourceAssembler.ToQueryFromResource(
                    edgeMac
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
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = e.Message }
                );
            }
        }

        [HttpGet("edge/{edgeMac}/registry")]
        public async Task<IActionResult> GetDeviceRegistry([FromRoute] string edgeMac)
        {
            try
            {
                var query = EdgeRegistryQueryFromResourceAssembler.ToQueryFromResource(edgeMac);
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
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = e.Message }
                );
            }
        }
    }
}
