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
public class RecommendationsController(
    IRecommendationCommandService recommendationCommandService,
    IRecommendationQueryService recommendationQueryService
) : ControllerBase
{
    [HttpGet("{plantationId:int}/recommendations/{recommendationId:int}")]
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
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Intervention not created. Invalid request.", typeof(string), ContentTypes = new []{"application/json"})]
    
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
