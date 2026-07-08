using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST;

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
        try
        {
            var query = new GetRecommendationByIdQuery(recommendationId);
            var recommendation = await recommendationQueryService.Handle(query);

            if (recommendation is null || recommendation.PlantationId != plantationId)
                return NotFound(new { message = "Recommendation not found." });

            var resource = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(
                recommendation
            );

            return Ok(resource);
        }
        catch (Exception e)
        {
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

            return Ok(resources);
        }
        catch (Exception e) when (e is ArgumentException)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
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

            return Created(
                $"/api/v1/plantations/{plantationId}/recommendations/{recommendation.Id}",
                response
            );
        }
        catch (Exception e) when (e is ArgumentException)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
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
        try
        {
            var existing = await recommendationQueryService.Handle(
                new GetRecommendationByIdQuery(recommendationId)
            );
            if (existing is null || existing.PlantationId != plantationId)
                return NotFound(new { message = "Recommendation not found." });

            var command = UpdateRecommendationContentCommandFromResourceAssembler.ToCommandFromResource(
                recommendationId,
                resource
            );

            var recommendation = await recommendationCommandService.Handle(command);

            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(
                recommendation
            );

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
        catch (Exception e)
        {
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
        try
        {
            var existing = await recommendationQueryService.Handle(
                new GetRecommendationByIdQuery(recommendationId)
            );
            if (existing is null || existing.PlantationId != plantationId)
                return NotFound(new { message = "Recommendation not found." });

            var command = new ApproveRecommendationCommand(recommendationId);
            var recommendation = await recommendationCommandService.Handle(command);

            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(
                recommendation
            );

            return Ok(response);
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            return Conflict(new { message = e.Message });
        }
        catch (Exception e)
        {
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
        try
        {
            var existing = await recommendationQueryService.Handle(
                new GetRecommendationByIdQuery(recommendationId)
            );
            if (existing is null || existing.PlantationId != plantationId)
                return NotFound(new { message = "Recommendation not found." });

            var command = new PublishRecommendationCommand(recommendationId);
            var recommendation = await recommendationCommandService.Handle(command);

            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(
                recommendation
            );

            return Ok(response);
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            return Conflict(new { message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPost("{plantationId:int}/recommendations/{recommendationId:int}/interventions")]
    [SwaggerOperation(
        Summary = "Registers an agronomic intervention for a specific recommendation.",
        Description = "This endpoint allows the registration of an agronomic intervention associated with a specific recommendation within a plantation. The intervention details are provided in the request body.",
        OperationId = "RegisterIntervention")]
    [SwaggerResponse(StatusCodes.Status201Created, "Intervention created", typeof(RegisterAgronomicInterventionResource), ContentTypes = new []{"application/json"})]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Intervention not created. Invalid request.")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The recommendation was not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "The intervention cannot be registered in the recommendation's current status")]
    public async Task<IActionResult> RegisterIntervention(
        [FromRoute] int plantationId,
        [FromRoute] int recommendationId,
        [FromBody] RegisterAgronomicInterventionResource resource
    )
    {
        try
        {
            var existing = await recommendationQueryService.Handle(
                new GetRecommendationByIdQuery(recommendationId)
            );
            if (existing is null || existing.PlantationId != plantationId)
                return NotFound(new { message = "Recommendation not found." });

            var command = RegisterAgronomicInterventionCommandFromResourceAssembler.ToCommandFromResource(
                recommendationId,
                resource
            );

            var intervention = await recommendationCommandService.Handle(command);

            var response = AgronomicInterventionResourceFromEntityAssembler.ToResourceFromEntity(
                intervention
            );

            return Created(
                $"/api/v1/plantations/{plantationId}/recommendations/{recommendationId}/interventions/{intervention.Id}",
                response
            );
        }
        catch (Exception e) when (e is ArgumentException)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e) when (e is InvalidOperationException)
        {
            return Conflict(new { message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpGet("{plantationId:int}/recommendations/{recommendationId:int}/interventions")]
    [SwaggerOperation(
        Summary = "Get the interventions of a recommendation",
        Description = "Returns the agronomic interventions registered for a specific recommendation.",
        OperationId = "GetInterventionsByRecommendationId")]
    [SwaggerResponse(StatusCodes.Status200OK, "The interventions were found", typeof(IEnumerable<AgronomicInterventionResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The recommendation was not found")]
    public async Task<IActionResult> GetInterventionsByRecommendationId(
        [FromRoute] int plantationId,
        [FromRoute] int recommendationId
    )
    {
        try
        {
            var existing = await recommendationQueryService.Handle(
                new GetRecommendationByIdQuery(recommendationId)
            );
            if (existing is null || existing.PlantationId != plantationId)
                return NotFound(new { message = "Recommendation not found." });

            var query = new GetInterventionsByRecommendationIdQuery(recommendationId);
            var interventions = await recommendationQueryService.Handle(query);

            var resources = interventions.Select(
                AgronomicInterventionResourceFromEntityAssembler.ToResourceFromEntity
            );

            return Ok(resources);
        }
        catch (Exception e)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = e.Message }
            );
        }
    }
}
