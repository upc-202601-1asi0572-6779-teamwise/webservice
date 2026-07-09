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
[Route("api/v1/plantations")]
[Produces(MediaTypeNames.Application.Json)]
[ApiController]
[SwaggerTag("Available Recommendation endpoints")]
public class RecommendationsController(
    IRecommendationCommandService recommendationCommandService,
    IRecommendationQueryService recommendationQueryService
) : ControllerBase
{
    [HttpGet("{plantationId:int}/recommendations/{recommendationId:int}")]
    [SwaggerOperation(
        Summary = "Get a recommendation by its id",
        Description = "Returns a single agronomic recommendation belonging to the given plantation.",
        OperationId = "GetRecommendationById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The recommendation was found", typeof(RecommendationResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The recommendation was not found")]
    public async Task<IActionResult> GetRecommendationById(
        [FromRoute] int plantationId,
        [FromRoute] int recommendationId
    )
    {
        Console.WriteLine($"[INFO] [BC] [Recommendations] GetRecommendationById called for plantationId: {plantationId}, recommendationId: {recommendationId}");
        try
        {
            var query = new GetRecommendationByIdQuery(recommendationId);
            var recommendation = await recommendationQueryService.Handle(query);

            if (recommendation is null || recommendation.PlantationId != plantationId)
            {
                Console.WriteLine($"[WARN] [BC] [Recommendations] Recommendation not found for plantationId: {plantationId}, recommendationId: {recommendationId}");
                return NotFound(new { message = "Recommendation not found." });
            }

            var resource = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(
                recommendation
            );

            Console.WriteLine($"[INFO] [BC] [Recommendations] Recommendation found for plantationId: {plantationId}, recommendationId: {recommendationId}");
            return Ok(resource);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [BC] [Recommendations] Error getting recommendation plantationId: {plantationId}, recommendationId: {recommendationId} - {e.Message}");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = e.Message }
            );
        }
    }

    [HttpGet("{plantationId:int}/recommendations")]
    [SwaggerOperation(
        Summary = "Get the recommendations of a plantation",
        Description = "Returns the agronomic recommendations of a plantation, optionally filtered by status and/or agronomist.",
        OperationId = "GetRecommendations")]
    [SwaggerResponse(StatusCodes.Status200OK, "The recommendations were found", typeof(IEnumerable<RecommendationResource>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid status filter")]
    public async Task<IActionResult> GetRecommendations(
        [FromRoute] int plantationId,
        [FromQuery] string? status,
        [FromQuery] int? agronomistId
    )
    {
        Console.WriteLine($"[INFO] [BC] [Recommendations] GetRecommendations called for plantationId: {plantationId}, status: {status}, agronomistId: {agronomistId}");
        try
        {
            var query = GetPlantationRecommendationsFromResourceAssembler.ToQueryFromResource(
                plantationId,
                status,
                agronomistId
            );
            var recommendations = await recommendationQueryService.Handle(query);

            var resources = recommendations.Select(
                RecommendationResourceFromEntityAssembler.ToResourceFromEntity
            );

            Console.WriteLine($"[INFO] [BC] [Recommendations] Retrieved {resources.Count()} recommendations for plantationId: {plantationId}");
            return Ok(resources);
        }
        catch (Exception e) when (e is ArgumentException)
        {
            Console.WriteLine($"[WARN] [BC] [Recommendations] Invalid status filter for plantationId: {plantationId} - {e.Message}");
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [BC] [Recommendations] Error getting recommendations for plantationId: {plantationId} - {e.Message}");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = e.Message }
            );
        }
    }

    [HttpPost("{plantationId:int}/recommendations")]
    [SwaggerOperation(
        Summary = "Create a recommendation",
        Description = "Registers a new agronomic recommendation for a plantation.",
        OperationId = "CreateRecommendation")]
    [SwaggerResponse(StatusCodes.Status201Created, "The recommendation was created", typeof(RecommendationResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid payload")]
    public async Task<IActionResult> CreateRecommendation(
        [FromRoute] int plantationId,
        [FromBody] CreateRecommendationResource resource
    )
    {
        Console.WriteLine($"[INFO] [BC] [Recommendations] CreateRecommendation called for plantationId: {plantationId}");
        try
        {
            var command = CreateRecommendationCommandFromResourceAssembler.ToCommandFromResource(
                plantationId,
                resource
            );

            var recommendation = await recommendationCommandService.Handle(command);

            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(
                recommendation
            );

            Console.WriteLine($"[INFO] [BC] [Recommendations] Recommendation created with id: {recommendation.Id} for plantationId: {plantationId}");
            return Created(
                $"/api/v1/plantations/{plantationId}/recommendations/{recommendation.Id}",
                response
            );
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
        {
            Console.WriteLine($"[WARN] [BC] [Recommendations] Validation failed creating recommendation for plantationId: {plantationId} - {e.Message}");
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [BC] [Recommendations] Error creating recommendation for plantationId: {plantationId} - {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPatch("{plantationId:int}/recommendations/{recommendationId:int}")]
    [SwaggerOperation(
        Summary = "Update the content of a recommendation",
        Description = "Partially updates the content of an existing agronomic recommendation.",
        OperationId = "UpdateRecommendationContent")]
    [SwaggerResponse(StatusCodes.Status200OK, "The recommendation was updated", typeof(RecommendationResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid payload")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The recommendation was not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "The recommendation cannot be updated in its current status")]
    public async Task<IActionResult> UpdateRecommendationContent(
        [FromRoute] int plantationId,
        [FromRoute] int recommendationId,
        [FromBody] UpdateRecommendationContentResource resource
    )
    {
        Console.WriteLine($"[INFO] [BC] [Recommendations] UpdateRecommendationContent called for plantationId: {plantationId}, recommendationId: {recommendationId}");
        try
        {
            var existing = await recommendationQueryService.Handle(
                new GetRecommendationByIdQuery(recommendationId)
            );
            if (existing is null || existing.PlantationId != plantationId)
            {
                Console.WriteLine($"[WARN] [BC] [Recommendations] Recommendation not found for update, plantationId: {plantationId}, recommendationId: {recommendationId}");
                return NotFound(new { message = "Recommendation not found." });
            }

            var command = UpdateRecommendationContentCommandFromResourceAssembler.ToCommandFromResource(
                recommendationId,
                resource
            );

            var recommendation = await recommendationCommandService.Handle(command);

            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(
                recommendation
            );

            Console.WriteLine($"[INFO] [BC] [Recommendations] Recommendation content updated for recommendationId: {recommendationId}");
            return Ok(response);
        }
        catch (Exception e) when (e is ArgumentException)
        {
            Console.WriteLine($"[WARN] [BC] [Recommendations] Invalid argument updating recommendation recommendationId: {recommendationId} - {e.Message}");
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            Console.WriteLine($"[WARN] [BC] [Recommendations] Invalid operation updating recommendation recommendationId: {recommendationId} - {e.Message}");
            return Conflict(new { message = e.Message });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [BC] [Recommendations] Error updating recommendation recommendationId: {recommendationId} - {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPatch("{plantationId:int}/recommendations/{recommendationId:int}/approval")]
    [SwaggerOperation(
        Summary = "Approve a recommendation",
        Description = "Transitions a recommendation to the approved status.",
        OperationId = "ApproveRecommendation")]
    [SwaggerResponse(StatusCodes.Status200OK, "The recommendation was approved", typeof(RecommendationResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The recommendation was not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "The recommendation cannot be approved in its current status")]
    public async Task<IActionResult> ApproveRecommendation(
        [FromRoute] int plantationId,
        [FromRoute] int recommendationId
    )
    {
        Console.WriteLine($"[INFO] [BC] [Recommendations] ApproveRecommendation called for plantationId: {plantationId}, recommendationId: {recommendationId}");
        try
        {
            var existing = await recommendationQueryService.Handle(
                new GetRecommendationByIdQuery(recommendationId)
            );
            if (existing is null || existing.PlantationId != plantationId)
            {
                Console.WriteLine($"[WARN] [BC] [Recommendations] Recommendation not found for approval, plantationId: {plantationId}, recommendationId: {recommendationId}");
                return NotFound(new { message = "Recommendation not found." });
            }

            var command = new ApproveRecommendationCommand(recommendationId);
            var recommendation = await recommendationCommandService.Handle(command);

            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(
                recommendation
            );

            Console.WriteLine($"[INFO] [BC] [Recommendations] Recommendation approved for recommendationId: {recommendationId}");
            return Ok(response);
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            Console.WriteLine($"[WARN] [BC] [Recommendations] Cannot approve recommendation recommendationId: {recommendationId} - {e.Message}");
            return Conflict(new { message = e.Message });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [BC] [Recommendations] Error approving recommendation recommendationId: {recommendationId} - {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPatch("{plantationId:int}/recommendations/{recommendationId:int}/publication")]
    [SwaggerOperation(
        Summary = "Publish a recommendation",
        Description = "Transitions an approved recommendation to the published status, making it visible to the producer.",
        OperationId = "PublishRecommendation")]
    [SwaggerResponse(StatusCodes.Status200OK, "The recommendation was published", typeof(RecommendationResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The recommendation was not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "The recommendation cannot be published in its current status")]
    public async Task<IActionResult> PublishRecommendation(
        [FromRoute] int plantationId,
        [FromRoute] int recommendationId
    )
    {
        Console.WriteLine($"[INFO] [BC] [Recommendations] PublishRecommendation called for plantationId: {plantationId}, recommendationId: {recommendationId}");
        try
        {
            var existing = await recommendationQueryService.Handle(
                new GetRecommendationByIdQuery(recommendationId)
            );
            if (existing is null || existing.PlantationId != plantationId)
            {
                Console.WriteLine($"[WARN] [BC] [Recommendations] Recommendation not found for publication, plantationId: {plantationId}, recommendationId: {recommendationId}");
                return NotFound(new { message = "Recommendation not found." });
            }

            var command = new PublishRecommendationCommand(recommendationId);
            var recommendation = await recommendationCommandService.Handle(command);

            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(
                recommendation
            );

            Console.WriteLine($"[INFO] [BC] [Recommendations] Recommendation published for recommendationId: {recommendationId}");
            return Ok(response);
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            Console.WriteLine($"[WARN] [BC] [Recommendations] Cannot publish recommendation recommendationId: {recommendationId} - {e.Message}");
            return Conflict(new { message = e.Message });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [BC] [Recommendations] Error publishing recommendation recommendationId: {recommendationId} - {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

}
