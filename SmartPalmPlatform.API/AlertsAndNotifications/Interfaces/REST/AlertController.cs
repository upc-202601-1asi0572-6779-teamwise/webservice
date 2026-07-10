using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Commands;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Queries;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.CommandServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.QueryServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Resources;
using SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Transform;
using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST;

[Authorize(Roles = "Administrator,PalmGrower,Agronomist")]
[RequireActiveSubscription]
[ApiController]
[Route("api/v1/alerts")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Alert & Notification endpoints")]
public class AlertController(
    IAlertCommandService alertCommandService,
    IAlertQueryService alertQueryService
) : ControllerBase
{
    private int GetCurrentUserId()
    {
        var user = HttpContext.Items["User"] as User;
        return user!.Id;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get my alerts",
        Description = "Returns all alerts for the authenticated user.",
        OperationId = "GetMyAlerts")]
    [SwaggerResponse(StatusCodes.Status200OK, "Alerts found", typeof(IEnumerable<AlertResource>))]
    public async Task<IActionResult> GetAlerts()
    {
        var query = new GetAlertsByUserIdQuery(GetCurrentUserId());
        var alerts = await alertQueryService.Handle(query);
        var resources = alerts.Select(AlertResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpPatch("{alertId:int}")]
    [SwaggerOperation(
        Summary = "Acknowledge an alert",
        Description = "Mark an alert as acknowledged.",
        OperationId = "AcknowledgeAlert")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Alert acknowledged")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Alert not found")]
    public async Task<IActionResult> AcknowledgeAlert(int alertId)
    {
        try
        {
            var command = new AcknowledgeAlertCommand(alertId);
            await alertCommandService.Handle(command);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = e.Message });
        }
    }
}