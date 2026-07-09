using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Queries;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.CommandServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.QueryServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Resources;
using SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Transform;
using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST;

[Authorize]
[ApiController]
[Route("api/v1/alerts")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Alert & Notification endpoints")]
public class AlertController(
    IAlertCommandService alertCommandService,
    IAlertQueryService alertQueryService) : ControllerBase
{
    private int GetCurrentUserId()
    {
        var user = HttpContext.Items["User"] as User;
        return user?.Id ?? 0;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get my alerts",
        Description = "Get all alerts for the authenticated user.",
        OperationId = "GetMyAlerts")]
    [SwaggerResponse(StatusCodes.Status200OK, "Alerts found", typeof(IEnumerable<AlertResource>))]
    public async Task<IActionResult> GetAlerts()
    {
        var userId = GetCurrentUserId();
        if (userId == 0) return Unauthorized(new { message = "User not authenticated." });
        Console.WriteLine($"[INFO] [Alerts] [AlertController] GetAlerts called with userId={userId}");
        try
        {
            var query = new GetAlertsByUserIdQuery(userId);
            var alerts = await alertQueryService.Handle(query);
            var resources = alerts.Select(AlertResourceFromEntityAssembler.ToResourceFromEntity);
            Console.WriteLine($"[INFO] [Alerts] [AlertController] Retrieved {resources.Count()} alerts for userId={userId}");
            return Ok(resources);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [Alerts] [AlertController] GetAlerts error for userId={userId}: {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPost("acknowledge")]
    [SwaggerOperation(
        Summary = "Acknowledge an alert",
        Description = "Mark an alert as acknowledged.",
        OperationId = "AcknowledgeAlert")]
    [SwaggerResponse(StatusCodes.Status200OK, "Alert acknowledged", typeof(AlertResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Alert not found")]
    public async Task<IActionResult> AcknowledgeAlert([FromBody] AcknowledgeAlertResource resource)
    {
        Console.WriteLine($"[INFO] [Alerts] [AlertController] AcknowledgeAlert called with alertId: {resource.alertId}");
        try
        {
            var command = AcknowledgeAlertCommandFromResourceAssembler.ToCommandFromResource(resource);
            var alert = await alertCommandService.Handle(command);
            var alertResource = AlertResourceFromEntityAssembler.ToResourceFromEntity(alert);
            Console.WriteLine($"[INFO] [Alerts] [AlertController] Alert acknowledged with id: {alert.Id}");
            return Ok(alertResource);
        }
        catch (KeyNotFoundException e)
        {
            Console.WriteLine($"[WARN] [Alerts] [AlertController] Alert not found for acknowledge, alertId: {resource.alertId} - {e.Message}");
            return NotFound(new { message = e.Message });
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
        {
            Console.WriteLine($"[WARN] [Alerts] [AlertController] Invalid acknowledge request for alertId: {resource.alertId} - {e.Message}");
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [Alerts] [AlertController] Error acknowledging alert alertId: {resource.alertId} - {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }
}
