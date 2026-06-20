using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Transform;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST;

[Route("api/v1")]
[ApiController]
public class RecommendationsController(
    IRecommendationCommandService recommendationCommandService,
    IRecommendationQueryService recommendationQueryService
) : ControllerBase
{
    [HttpGet(
        "agronomist/{agronomistId:int}/plantation/{plantationId:int}/recomendations/{recommendationId:int}"
    )]
    public async Task<IActionResult> GetRecommendationById(
        [FromRoute] int agronomistId,
        [FromRoute] int plantationId,
        [FromRoute] int recommendationId
    )
    {
        try
        {
            var query = new GetAgronomistPlantationRecomendationByIdQuery(
                agronomistId,
                plantationId,
                recommendationId
            );
            var recommendation = await recommendationQueryService.Handle(query);

            if (recommendation is null)
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

    [HttpGet("agronomist/{agronomistId:int}/plantation/{plantationId:int}/recomendations")]
    public async Task<IActionResult> GetPendingRecommendations(
        [FromRoute] int agronomistId,
        [FromRoute] int plantationId,
        [FromQuery] string? status
    )
    {
        try
        {
            var query =
                GetAgranomistPlantationRecomendationsByStatusFromResourceAssembler.ToQueryFromResource(
                    agronomistId,
                    plantationId,
                    status
                );
            var recommendations = await recommendationQueryService.Handle(query);

            var resources = recommendations.Select(
                RecommendationResourceFromEntityAssembler.ToResourceFromEntity
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

    [HttpPost("agronomist/{agronomistId:int}/plantation/{plantationId:int}/recomendations")]
    public async Task<IActionResult> CreateRecommendation(
        [FromRoute] int agronomistId,
        [FromRoute] int plantationId,
        [FromBody] CreateRecommendationResource resource
    )
    {
        try
        {
            var command = CreateRecommendationCommandFromResourceAssembler.ToCommandFromResource(
                plantationId,
                agronomistId,
                resource
            );

            var recommendation = await recommendationCommandService.Handle(command);

            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(
                recommendation
            );

            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPut(
        "agronomist/{agronomistId:int}/plantation/{plantationId:int}/recomendations/{recommendationId:int}"
    )]
    public async Task<IActionResult> UpdateRecommendationContent(
        [FromRoute] int agronomistId,
        [FromRoute] int plantationId,
        [FromRoute] int recommendationId,
        [FromBody] UpdateRecommendationContentResource resource
    )
    {
        try
        {
            var command =
                UpdateRecommendationContentCommandFromResourceAssembler.ToCommandFromResource(
                    agronomistId,
                    plantationId,
                    recommendationId,
                    resource
                );

            var recommendation = await recommendationCommandService.Handle(command);

            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(
                recommendation
            );

            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPatch(
        "agronomist/{agronomistId:int}/plantation/{plantationId:int}/recomendations/{recommendationId:int}/aproval"
    )]
    public async Task<IActionResult> ApproveRecommendation(
        [FromRoute] int agronomistId,
        [FromRoute] int plantationId,
        [FromRoute] int recommendationId
    )
    {
        try
        {
            var command = new ApproveRecommendationCommand(
                agronomistId,
                plantationId,
                recommendationId
            );
            var recommendation = await recommendationCommandService.Handle(command);

            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(
                recommendation
            );

            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPatch(
        "agronomist/{agronomistId:int}/plantation/{plantationId:int}/recomendations/{recommendationId:int}/publishing"
    )]
    public async Task<IActionResult> PublishRecommendation(
        [FromRoute] int agronomistId,
        [FromRoute] int plantationId,
        [FromRoute] int recommendationId
    )
    {
        try
        {
            var command = new PublishRecommendationCommand(
                agronomistId,
                plantationId,
                recommendationId
            );
            var recommendation = await recommendationCommandService.Handle(command);

            var response = RecommendationResourceFromEntityAssembler.ToResourceFromEntity(
                recommendation
            );

            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost(
        "agronomist/{agronomistId:int}/plantation/{plantationId:int}/recomendations/{recommendationId:int}/interventions"
    )]
    public async Task<IActionResult> RegisterIntervention(
        [FromRoute] int agronomistId,
        [FromRoute] int plantationId,
        [FromRoute] int recommendationId,
        [FromBody] RegisterAgronomicInterventionResource resource
    )
    {
        try
        {
            var command =
                RegisterAgronomicInterventionCommandFromResourceAssembler.ToCommandFromResource(
                    agronomistId,
                    plantationId,
                    recommendationId,
                    resource
                );

            var intervention = await recommendationCommandService.Handle(command);

            var response = AgronomicInterventionResourceFromEntityAssembler.ToResourceFromEntity(
                intervention
            );

            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpGet(
        "agronomist/{agronomistId:int}/plantation/{plantationId:int}/recomendations/{recommendationId:int}/interventions"
    )]
    public async Task<IActionResult> GetInterventionsByRecommendationId(
        [FromRoute] int agronomistId,
        [FromRoute] int plantationId,
        [FromRoute] int recommendationId
    )
    {
        try
        {
            var query = new GetInterventionsByRecommendationIdQuery(
                agronomistId,
                plantationId,
                recommendationId
            );
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

