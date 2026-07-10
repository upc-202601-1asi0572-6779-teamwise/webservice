using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST;

[Authorize]
[RequireActiveSubscription]
[Route("api/v1")]
[Produces(MediaTypeNames.Application.Json)]
[ApiController]
[SwaggerTag("Report endpoints")]
public class ReportsController(
    IReportCommandService reportCommandService,
    IReportQueryService reportQueryService
) : ControllerBase
{
    [HttpGet("sectors/{sectorId:int}/reports")]
    [SwaggerOperation(
        Summary = "List reports by sector",
        Description = "Returns all reports for a specific sector.",
        OperationId = "ListSectorReports")]
    [SwaggerResponse(StatusCodes.Status200OK, "Reports found", typeof(IEnumerable<ReportResource>))]
    public async Task<IActionResult> ListSectorReports(int sectorId)
    {
        var query = new GetSectorReportsQuery(sectorId);
        var reports = await reportQueryService.Handle(query);
        var resources = reports.Select(ReportResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("reports/{reportId:int}")]
    [SwaggerOperation(
        Summary = "Get a report by ID",
        Description = "Returns a single report.",
        OperationId = "GetReportById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Report found", typeof(ReportResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Report not found")]
    public async Task<IActionResult> GetReportById(int reportId)
    {
        var query = new GetReportByIdQuery(reportId);
        var report = await reportQueryService.Handle(query);
        if (report is null)
            return NotFound(new { message = "Report not found." });
        var resource = ReportResourceFromEntityAssembler.ToResourceFromEntity(report);
        return Ok(resource);
    }

    [HttpPost("sectors/{sectorId:int}/reports")]
    [SwaggerOperation(
        Summary = "Create a report",
        Description = "Registers a new report for a specific sector.",
        OperationId = "CreateReport")]
    [SwaggerResponse(StatusCodes.Status201Created, "Report created", typeof(ReportResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid payload")]
    public async Task<IActionResult> CreateReport(
        int sectorId,
        [FromBody] CreateReportResource resource)
    {
        try
        {
            var command = CreateReportCommandFromResourceAssembler.ToCommandFromResource(sectorId, resource);
            var report = await reportCommandService.Handle(command);
            var response = ReportResourceFromEntityAssembler.ToResourceFromEntity(report);
            return Created($"/api/v1/reports/{report.Id}", response);
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPatch("reports/{reportId:int}")]
    [SwaggerOperation(
        Summary = "Update report content",
        Description = "Partially updates the content of an existing report.",
        OperationId = "UpdateReportContent")]
    [SwaggerResponse(StatusCodes.Status200OK, "Report updated", typeof(ReportResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid payload")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Report not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Cannot update in current status")]
    public async Task<IActionResult> UpdateReportContent(
        int reportId,
        [FromBody] UpdateReportContentResource resource)
    {
        try
        {
            var command = UpdateReportContentCommandFromResourceAssembler.ToCommandFromResource(reportId, resource);
            var report = await reportCommandService.Handle(command);
            var response = ReportResourceFromEntityAssembler.ToResourceFromEntity(report);
            return Ok(response);
        }
        catch (Exception e) when (e is KeyNotFoundException)
        {
            return NotFound(new { message = e.Message });
        }
        catch (Exception e) when (e is ArgumentException)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            return Conflict(new { message = e.Message });
        }
    }

    [HttpPatch("reports/{reportId:int}/publication")]
    [Authorize(Roles = "Administrator,Agronomist")]
    [SwaggerOperation(
        Summary = "Publish a report",
        Description = "Transitions a draft report to the published status.",
        OperationId = "PublishReport")]
    [SwaggerResponse(StatusCodes.Status200OK, "Report published", typeof(ReportResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Report not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Cannot publish in current status")]
    public async Task<IActionResult> PublishReport(int reportId)
    {
        try
        {
            var command = new PublishReportCommand(reportId);
            var report = await reportCommandService.Handle(command);
            var response = ReportResourceFromEntityAssembler.ToResourceFromEntity(report);
            return Ok(response);
        }
        catch (Exception e) when (e is KeyNotFoundException)
        {
            return NotFound(new { message = e.Message });
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            return Conflict(new { message = e.Message });
        }
    }
}