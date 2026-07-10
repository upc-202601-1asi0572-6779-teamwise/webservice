using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.CommandServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.QueryServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Resources;
using SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Transform;
using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.DomainServices;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST;

[Authorize]
[RequireActiveSubscription]
[ApiController]
[Route("api/v1/alert-settings")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("User Alert Settings")]
public class UserAlertSettingsController(
    IUserAlertSettingCommandService commandService,
    IUserAlertSettingQueryService queryService,
    ISensorTypeDomainService sensorTypeDomainService) : ControllerBase
{
    private int GetCurrentUserId()
    {
        var user = HttpContext.Items["User"] as User;
        return user!.Id;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List my alert settings", OperationId = "GetMyAlertSettings")]
    [SwaggerResponse(StatusCodes.Status200OK, "Settings found", typeof(IEnumerable<UserAlertSettingResource>))]
    public async Task<IActionResult> GetAll()
    {
        var settings = await queryService.Handle(GetCurrentUserId());
        var resources = settings.Select(UserAlertSettingResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("{sensorType}")]
    [SwaggerOperation(Summary = "Get alert setting by sensor type", OperationId = "GetMyAlertSettingBySensorType")]
    [SwaggerResponse(StatusCodes.Status200OK, "Setting found", typeof(UserAlertSettingResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Setting not found")]
    public async Task<IActionResult> GetBySensorType(string sensorType)
    {
        if (!sensorTypeDomainService.TryParseSensorType(sensorType, out var parsed))
            return BadRequest(new { message = $"Invalid sensor type: {sensorType}" });

        var setting = await queryService.Handle(GetCurrentUserId(), parsed);
        if (setting is null)
            return NotFound(new { message = "Setting not found for this sensor type." });

        return Ok(UserAlertSettingResourceFromEntityAssembler.ToResourceFromEntity(setting));
    }

    [HttpPut("{sensorType}")]
    [SwaggerOperation(Summary = "Create or update alert setting", OperationId = "UpdateMyAlertSetting")]
    [SwaggerResponse(StatusCodes.Status200OK, "Setting updated", typeof(UserAlertSettingResource))]
    public async Task<IActionResult> Update(string sensorType, [FromBody] UpdateUserAlertSettingResource resource)
    {
        if (!sensorTypeDomainService.TryParseSensorType(sensorType, out var parsed))
            return BadRequest(new { message = $"Invalid sensor type: {sensorType}" });

        var setting = await commandService.Handle(GetCurrentUserId(), parsed, resource.isMuted);
        return Ok(UserAlertSettingResourceFromEntityAssembler.ToResourceFromEntity(setting));
    }
}
