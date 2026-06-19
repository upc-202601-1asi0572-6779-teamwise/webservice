using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Transform;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST;

[Route("api/v1/agronomic-recommendations")]
[ApiController]
public class RecommendationsController(
    IRecommendationCommandService recommendationCommandService,
    IRecommendationQueryService recommendationQueryService
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllRecommendations()
    {
        try
        {
            var query = new GetAllRecommendationsQuery();
            var recommendations = await recommendationQueryService.Handle(query);

            var resources = recommendations
                .Select(RecommendationResourceFromEntityAssembler.ToResourceFromEntity);

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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRecommendationById(int id)
    {
        try
        {
            var query = new GetRecommendationByIdQuery(id);
            var recommendation = await recommendationQueryService.Handle(query);

            if (recommendation is null)
                return NotFound(new { message = "Recommendation not found." });

            var resource =
                RecommendationResourceFromEntityAssembler.ToResourceFromEntity(recommendation);

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

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingRecommendations()
    {
        try
        {
            var query = new GetPendingRecommendationsQuery();
            var recommendations = await recommendationQueryService.Handle(query);

            var resources = recommendations
                .Select(RecommendationResourceFromEntityAssembler.ToResourceFromEntity);

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

    [HttpGet("plantation/{plantationId:int}")]
    public async Task<IActionResult> GetRecommendationsByPlantationId(int plantationId)
    {
        try
        {
            var query = new GetRecommendationsByPlantationIdQuery(plantationId);
            var recommendations = await recommendationQueryService.Handle(query);

            var resources = recommendations
                .Select(RecommendationResourceFromEntityAssembler.ToResourceFromEntity);

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

    [HttpPost]
    public async Task<IActionResult> CreateRecommendation(
        [FromBody] CreateRecommendationResource resource
    )
    {
        try
        {
            var command =
                CreateRecommendationCommandFromResourceAssembler.ToCommandFromResource(resource);

            var recommendation = await recommendationCommandService.Handle(command);

            var response =
                RecommendationResourceFromEntityAssembler.ToResourceFromEntity(recommendation);

            return CreatedAtAction(
                nameof(GetRecommendationById),
                new { id = response.id },
                response
            );
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateRecommendationContent(
        int id,
        [FromBody] UpdateRecommendationContentResource resource
    )
    {
        try
        {
            var command =
                UpdateRecommendationContentCommandFromResourceAssembler.ToCommandFromResource(
                    id,
                    resource
                );

            var recommendation = await recommendationCommandService.Handle(command);

            var response =
                RecommendationResourceFromEntityAssembler.ToResourceFromEntity(recommendation);

            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPatch("{id:int}/approve")]
    public async Task<IActionResult> ApproveRecommendation(int id)
    {
        try
        {
            var command = new ApproveRecommendationCommand(id);
            var recommendation = await recommendationCommandService.Handle(command);

            var response =
                RecommendationResourceFromEntityAssembler.ToResourceFromEntity(recommendation);

            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPatch("{id:int}/publish")]
    public async Task<IActionResult> PublishRecommendation(int id)
    {
        try
        {
            var command = new PublishRecommendationCommand(id);
            var recommendation = await recommendationCommandService.Handle(command);

            var response =
                RecommendationResourceFromEntityAssembler.ToResourceFromEntity(recommendation);

            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost("{id:int}/interventions")]
    public async Task<IActionResult> RegisterIntervention(
        int id,
        [FromBody] RegisterAgronomicInterventionResource resource
    )
    {
        try
        {
            var command =
                RegisterAgronomicInterventionCommandFromResourceAssembler.ToCommandFromResource(
                    id,
                    resource
                );

            var intervention = await recommendationCommandService.Handle(command);

            var response =
                AgronomicInterventionResourceFromEntityAssembler.ToResourceFromEntity(intervention);

            return Ok(response);
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpGet("{id:int}/interventions")]
    public async Task<IActionResult> GetInterventionsByRecommendationId(int id)
    {
        try
        {
            var query = new GetInterventionsByRecommendationIdQuery(id);
            var interventions = await recommendationQueryService.Handle(query);

            var resources = interventions
                .Select(AgronomicInterventionResourceFromEntityAssembler.ToResourceFromEntity);

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