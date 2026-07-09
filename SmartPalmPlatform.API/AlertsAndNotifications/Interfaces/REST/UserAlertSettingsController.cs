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
        Console.WriteLine($"[INFO] [BC] [UserAlertSettings] GetAll called with userId: {userId}");
        try
        {
            var settings = await queryService.Handle(userId);
            var resources = settings.Select(UserAlertSettingResourceFromEntityAssembler.ToResourceFromEntity);
            Console.WriteLine($"[INFO] [BC] [UserAlertSettings] Retrieved {resources.Count()} alert settings for userId: {userId}");
            return Ok(resources);
        }
        catch (KeyNotFoundException e)
        {
            Console.WriteLine($"[WARN] [BC] [UserAlertSettings] User not found for alert settings, userId: {userId} - {e.Message}");
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [BC] [UserAlertSettings] Error getting alert settings for userId: {userId} - {e.Message}");
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
        Console.WriteLine($"[INFO] [BC] [UserAlertSettings] GetBySensorType called with userId: {userId}, sensorType: {sensorType}");
        try
        {
            if (!Enum.TryParse<SensorType>(sensorType, ignoreCase: true, out var parsed))
            {
                Console.WriteLine($"[WARN] [BC] [UserAlertSettings] Invalid sensor type: {sensorType} for userId: {userId}");
                return BadRequest(new { message = $"Invalid sensor type: {sensorType}" });
            }

            var setting = await queryService.Handle(userId, parsed);
            if (setting is null)
            {
                Console.WriteLine($"[WARN] [BC] [UserAlertSettings] Setting not found for sensorType: {sensorType}, userId: {userId}");
                return NotFound(new { message = "Setting not found for this sensor type." });
            }

            var resource = UserAlertSettingResourceFromEntityAssembler.ToResourceFromEntity(setting);
            Console.WriteLine($"[INFO] [BC] [UserAlertSettings] Alert setting found for userId: {userId}, sensorType: {sensorType}");
            return Ok(resource);
        }
        catch (KeyNotFoundException e)
        {
            Console.WriteLine($"[WARN] [BC] [UserAlertSettings] User not found for alert setting, userId: {userId} - {e.Message}");
            return NotFound(new { message = e.Message });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [BC] [UserAlertSettings] Error getting alert setting for userId: {userId}, sensorType: {sensorType} - {e.Message}");
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
        Console.WriteLine($"[INFO] [BC] [UserAlertSettings] Update called with userId: {userId}, sensorType: {sensorType}, isMuted: {resource.isMuted}");
        try
        {
            if (!Enum.TryParse<SensorType>(sensorType, ignoreCase: true, out var parsed))
            {
                Console.WriteLine($"[WARN] [BC] [UserAlertSettings] Invalid sensor type: {sensorType} for userId: {userId}");
                return BadRequest(new { message = $"Invalid sensor type: {sensorType}" });
            }

            var setting = await commandService.Handle(userId, parsed, resource.isMuted);
            var output = UserAlertSettingResourceFromEntityAssembler.ToResourceFromEntity(setting);
            Console.WriteLine($"[INFO] [BC] [UserAlertSettings] Alert setting updated for userId: {userId}, sensorType: {sensorType}");
            return Ok(output);
        }
        catch (KeyNotFoundException e)
        {
            Console.WriteLine($"[WARN] [BC] [UserAlertSettings] User not found for alert setting update, userId: {userId} - {e.Message}");
            return NotFound(new { message = e.Message });
        }
        catch (ArgumentException e)
        {
            Console.WriteLine($"[WARN] [BC] [UserAlertSettings] Invalid argument updating alert setting for userId: {userId} - {e.Message}");
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [BC] [UserAlertSettings] Error updating alert setting for userId: {userId}, sensorType: {sensorType} - {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }
}
