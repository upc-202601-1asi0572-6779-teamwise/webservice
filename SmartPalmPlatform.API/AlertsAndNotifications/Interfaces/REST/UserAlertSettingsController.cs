using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.CommandServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.QueryServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Resources;
using SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Transform;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST;

[Authorize]
[ApiController]
[Route("api/v1/users/{userId}/alert-settings")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("User Alert Settings")]
public class UserAlertSettingsController(
    IUserAlertSettingCommandService commandService,
    IUserAlertSettingQueryService queryService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "List alert settings for a user",
        Description = "Get all alert settings for the specified user.",
        OperationId = "GetUserAlertSettings")]
    [SwaggerResponse(StatusCodes.Status200OK, "Settings found", typeof(IEnumerable<UserAlertSettingResource>))]
    public async Task<IActionResult> GetAll(int userId)
    {
        try
        {
            var settings = await queryService.Handle(userId);
            var resources = settings.Select(UserAlertSettingResourceFromEntityAssembler.ToResourceFromEntity);
            return Ok(resources);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpGet("{sensorType}")]
    [SwaggerOperation(
        Summary = "Get alert setting by sensor type",
        Description = "Get the alert setting for a specific sensor type.",
        OperationId = "GetUserAlertSettingBySensorType")]
    [SwaggerResponse(StatusCodes.Status200OK, "Setting found", typeof(UserAlertSettingResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Setting not found")]
    public async Task<IActionResult> GetBySensorType(int userId, string sensorType)
    {
        try
        {
            if (!Enum.TryParse<SensorType>(sensorType, ignoreCase: true, out var parsed))
                return BadRequest(new { message = $"Invalid sensor type: {sensorType}" });

            var setting = await queryService.Handle(userId, parsed);
            if (setting is null)
                return NotFound(new { message = "Setting not found for this sensor type." });

            var resource = UserAlertSettingResourceFromEntityAssembler.ToResourceFromEntity(setting);
            return Ok(resource);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPut("{sensorType}")]
    [SwaggerOperation(
        Summary = "Create or update alert setting",
        Description = "Set mute/unmute for a sensor type. Creates the setting if it doesn't exist.",
        OperationId = "UpdateUserAlertSetting")]
    [SwaggerResponse(StatusCodes.Status200OK, "Setting updated", typeof(UserAlertSettingResource))]
    public async Task<IActionResult> Update(int userId, string sensorType, [FromBody] UpdateUserAlertSettingResource resource)
    {
        try
        {
            if (!Enum.TryParse<SensorType>(sensorType, ignoreCase: true, out var parsed))
                return BadRequest(new { message = $"Invalid sensor type: {sensorType}" });

            var setting = await commandService.Handle(userId, parsed, resource.isMuted);
            var output = UserAlertSettingResourceFromEntityAssembler.ToResourceFromEntity(setting);
            return Ok(output);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }
}
