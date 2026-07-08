using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Queries;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.CommandServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.QueryServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Resources;
using SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Transform;
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
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get alerts by user",
        Description = "Get all alerts for the specified user.",
        OperationId = "GetAlertsByUser")]
    [SwaggerResponse(StatusCodes.Status200OK, "Alerts found", typeof(IEnumerable<AlertResource>))]
    public async Task<IActionResult> GetAlerts([FromQuery] int userId)
    {
        try
        {
            var query = new GetAlertsByUserIdQuery(userId);
            var alerts = await alertQueryService.Handle(query);
            var resources = alerts.Select(AlertResourceFromEntityAssembler.ToResourceFromEntity);
            return Ok(resources);
        }
        catch (Exception e)
        {
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
        try
        {
            var command = AcknowledgeAlertCommandFromResourceAssembler.ToCommandFromResource(resource);
            var alert = await alertCommandService.Handle(command);
            var alertResource = AlertResourceFromEntityAssembler.ToResourceFromEntity(alert);
            return Ok(alertResource);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }
}
