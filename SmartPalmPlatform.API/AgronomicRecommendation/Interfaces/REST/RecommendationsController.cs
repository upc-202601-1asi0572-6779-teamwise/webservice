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
[SwaggerTag("Recommendation endpoints")]
public class RecommendationsController(
    IRecommendationCommandService recommendationCommandService,
    IRecommendationQueryService recommendationQueryService
) : ControllerBase
{
    [HttpGet("sectors/{sectorId:int}/recommendations")]
    [SwaggerOperation(
        Summary = "List recommendations by sector",
        Description = "Returns recommendations for a sector, optionally filtered by status and/or agronomist.",
        OperationId = "ListSectorRecommendations")]
    [SwaggerResponse(StatusCodes.Status200OK, "Recommendations found", typeof(IEnumerable<RecommendationResource>))]
    public async Task<IActionResult> ListSectorRecommendations(
        int sectorId,
        [FromQuery] string? status,
        [FromQuery] int? agronomistId)
    {
        var query = GetSectorRecommendationsFromResourceAssembler.ToQueryFromResource(sectorId, status, agronomistId);
        var recommendations = await recommendationQueryService.Handle(query);
        var resources = recommendations.Select(RecommendationResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpPost("sectors/{sectorId:int}/recommendations")]
    [SwaggerOperation(
        Summary = "Create a sector recommendation",
        Description = "Registers a new agronomic recommendation for a specific sector.",
        OperationId = "CreateSectorRecommendation")]
    [SwaggerResponse(StatusCodes.Status201Created, "Recommendation created", typeof(RecommendationResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid payload")]
    public async Task<IActionResult> CreateSectorRecommendation(
        int sectorId,
        [FromBody] CreateRecommendationResource resource)
    {
        try
        {
            var command = CreateRecommendationCommandFromResourceAssembler.ToCommandFromResource(sectorId, resource);
            var recommendation = await recommendationCommandService.Handle(command);
            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(recommendation);
            return Created($"/api/v1/recommendations/{recommendation.Id}", response);
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpGet("recommendations/general")]
    [SwaggerOperation(
        Summary = "List general recommendations",
        Description = "Returns all general (non-sector) recommendations, optionally filtered by status.",
        OperationId = "ListGeneralRecommendations")]
    [SwaggerResponse(StatusCodes.Status200OK, "Recommendations found", typeof(IEnumerable<RecommendationResource>))]
    public async Task<IActionResult> ListGeneralRecommendations(
        [FromQuery] string? status,
        [FromQuery] int? agronomistId)
    {
        var query = GetSectorRecommendationsFromResourceAssembler.ToGeneralQueryFromResource(status, agronomistId);
        var recommendations = await recommendationQueryService.Handle(query);
        var resources = recommendations.Select(RecommendationResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpPost("recommendations/general")]
    [SwaggerOperation(
        Summary = "Create a general recommendation",
        Description = "Registers a new general agronomic recommendation (not tied to a sector).",
        OperationId = "CreateGeneralRecommendation")]
    [SwaggerResponse(StatusCodes.Status201Created, "Recommendation created", typeof(RecommendationResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid payload")]
    public async Task<IActionResult> CreateGeneralRecommendation(
        [FromBody] CreateRecommendationResource resource)
    {
        try
        {
            var command = CreateRecommendationCommandFromResourceAssembler.ToCommandFromResource(null, resource);
            var recommendation = await recommendationCommandService.Handle(command);
            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(recommendation);
            return Created($"/api/v1/recommendations/{recommendation.Id}", response);
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpGet("reports/{reportId:int}/recommendations")]
    [SwaggerOperation(
        Summary = "List recommendations by report",
        Description = "Returns recommendations for a specific report.",
        OperationId = "ListReportRecommendations")]
    [SwaggerResponse(StatusCodes.Status200OK, "Recommendations found", typeof(IEnumerable<RecommendationResource>))]
    public async Task<IActionResult> ListReportRecommendations(int reportId)
    {
        var query = new GetRecommendationsByReportIdQuery(reportId);
        var recommendations = await recommendationQueryService.Handle(query);
        var resources = recommendations.Select(RecommendationResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("recommendations/{recommendationId:int}")]
    [SwaggerOperation(
        Summary = "Get a recommendation by ID",
        Description = "Returns a single agronomic recommendation.",
        OperationId = "GetRecommendationById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Recommendation found", typeof(RecommendationResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Recommendation not found")]
    public async Task<IActionResult> GetRecommendationById(int recommendationId)
    {
        var query = new GetRecommendationByIdQuery(recommendationId);
        var recommendation = await recommendationQueryService.Handle(query);
        if (recommendation is null)
            return NotFound(new { message = "Recommendation not found." });
        var resource = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(recommendation);
        return Ok(resource);
    }

    [HttpPatch("recommendations/{recommendationId:int}")]
    [SwaggerOperation(
        Summary = "Update recommendation content",
        Description = "Partially updates the content of an existing recommendation.",
        OperationId = "UpdateRecommendationContent")]
    [SwaggerResponse(StatusCodes.Status200OK, "Recommendation updated", typeof(RecommendationResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid payload")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Recommendation not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Cannot update in current status")]
    public async Task<IActionResult> UpdateRecommendationContent(
        int recommendationId,
        [FromBody] UpdateRecommendationContentResource resource)
    {
        try
        {
            var command = UpdateRecommendationContentCommandFromResourceAssembler.ToCommandFromResource(recommendationId, resource);
            var recommendation = await recommendationCommandService.Handle(command);
            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(recommendation);
            return Ok(response);
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

    [HttpPatch("recommendations/{recommendationId:int}/approval")]
    [Authorize(Roles = "Administrator,Agronomist")]
    [SwaggerOperation(
        Summary = "Approve a recommendation",
        Description = "Transitions a recommendation to the approved status.",
        OperationId = "ApproveRecommendation")]
    [SwaggerResponse(StatusCodes.Status200OK, "Recommendation approved", typeof(RecommendationResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Recommendation not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Cannot approve in current status")]
    public async Task<IActionResult> ApproveRecommendation(int recommendationId)
    {
        try
        {
            var command = new ApproveRecommendationCommand(recommendationId);
            var recommendation = await recommendationCommandService.Handle(command);
            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(recommendation);
            return Ok(response);
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            return Conflict(new { message = e.Message });
        }
    }

    [HttpPatch("recommendations/{recommendationId:int}/publication")]
    [Authorize(Roles = "Administrator,Agronomist")]
    [SwaggerOperation(
        Summary = "Publish a recommendation",
        Description = "Transitions an approved recommendation to the published status.",
        OperationId = "PublishRecommendation")]
    [SwaggerResponse(StatusCodes.Status200OK, "Recommendation published", typeof(RecommendationResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Recommendation not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Cannot publish in current status")]
    public async Task<IActionResult> PublishRecommendation(int recommendationId)
    {
        try
        {
            var command = new PublishRecommendationCommand(recommendationId);
            var recommendation = await recommendationCommandService.Handle(command);
            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(recommendation);
            return Ok(response);
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            return Conflict(new { message = e.Message });
        }
    }
}