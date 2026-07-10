using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Queries;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.QueryServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Resources;
using SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Transform;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Admin;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/v1/admin/alerts")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Admin Alert management")]
public class AdminAlertsController(
    IAlertQueryService alertQueryService
) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "List all alerts",
        Description = "Returns all alerts in the platform.",
        OperationId = "AdminListAlerts")]
    [SwaggerResponse(StatusCodes.Status200OK, "Alerts found", typeof(IEnumerable<AlertResource>))]
    public async Task<IActionResult> ListAlerts()
    {
        var query = new GetAllAlertsQuery();
        var alerts = await alertQueryService.Handle(query);
        var resources = alerts.Select(AlertResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}