using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Queries;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Services;
using SmartPalmPlatform.API.FieldTechnicalManagement.Interfaces.REST.Resources;
using SmartPalmPlatform.API.FieldTechnicalManagement.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.FieldTechnicalManagement.Interfaces.REST;

[Authorize]
[RequireActiveSubscription]
[Route("api/v1")]
[Produces(MediaTypeNames.Application.Json)]
[ApiController]
[SwaggerTag("Field Technical Management - Intervention endpoints")]
public class AgronomicInterventionController(
    IAgronomicInterventionCommandService commandService,
    IAgronomicInterventionQueryService queryService
) : ControllerBase
{
    [HttpPost("sectors/{sectorId:int}/interventions")]
    [SwaggerOperation(
        Summary = "Register a new agronomic intervention",
        Description = "Registers a new agronomic intervention for a specific sector.",
        OperationId = "RegisterIntervention")]
    [SwaggerResponse(StatusCodes.Status201Created, "Intervention created", typeof(AgronomicInterventionResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
    public async Task<IActionResult> RegisterIntervention(
        int sectorId,
        [FromBody] RegisterInterventionResource resource)
    {
        try
        {
            var command = RegisterCommandFromResourceAssembler.ToCommandFromResource(sectorId, resource);
            var intervention = await commandService.Handle(command);
            var response = AgronomicInterventionResourceFromEntityAssembler.ToResourceFromEntity(intervention);
            return Created($"/api/v1/sectors/{sectorId}/interventions/{intervention.Id}", response);
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpGet("sectors/{sectorId:int}/interventions")]
    [SwaggerOperation(
        Summary = "List interventions by sector",
        Description = "Returns all interventions for a specific sector.",
        OperationId = "ListInterventionsBySector")]
    [SwaggerResponse(StatusCodes.Status200OK, "Interventions found", typeof(IEnumerable<AgronomicInterventionResource>))]
    public async Task<IActionResult> ListInterventionsBySector(int sectorId)
    {
        var query = new GetAgronomicInterventionsBySectorIdQuery(sectorId);
        var interventions = await queryService.Handle(query);
        var resources = interventions.Select(AgronomicInterventionResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("interventions/{interventionId:int}")]
    [SwaggerOperation(
        Summary = "Get intervention by ID",
        Description = "Returns the detail of a specific intervention with its complete traceability.",
        OperationId = "GetAgronomicInterventionById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Intervention found", typeof(AgronomicInterventionResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Intervention not found")]
    public async Task<IActionResult> GetAgronomicInterventionById(int interventionId)
    {
        var query = new GetAgronomicInterventionByIdQuery(interventionId);
        var intervention = await queryService.Handle(query);
        if (intervention is null)
            return NotFound(new { message = "Intervention not found." });
        var resource = AgronomicInterventionResourceFromEntityAssembler.ToResourceFromEntity(intervention);
        return Ok(resource);
    }

    [HttpGet("plantations/{plantationId:int}/interventions")]
    [SwaggerOperation(
        Summary = "List interventions by plantation",
        Description = "Returns interventions executed within any sector of the given plantation. Supports optional date-range filter.",
        OperationId = "ListPlantationInterventions")]
    [SwaggerResponse(StatusCodes.Status200OK, "Interventions found", typeof(IEnumerable<AgronomicInterventionResource>))]
    public async Task<IActionResult> ListInterventionsByPlantation(
        int plantationId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var query = new GetAgronomicInterventionsByPlantationQuery(plantationId, from, to);
        var interventions = await queryService.Handle(query);
        var resources = interventions.Select(AgronomicInterventionResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("recommendations/{recommendationId:int}/interventions")]
    [SwaggerOperation(
        Summary = "List interventions by recommendation",
        Description = "Returns interventions whose OriginRecommendationId matches the given recommendation.",
        OperationId = "ListRecommendationInterventions")]
    [SwaggerResponse(StatusCodes.Status200OK, "Interventions found", typeof(IEnumerable<AgronomicInterventionResource>))]
    public async Task<IActionResult> ListInterventionsByRecommendation(int recommendationId)
    {
        var query = new GetAgronomicInterventionsByRecommendationIdQuery(recommendationId);
        var interventions = await queryService.Handle(query);
        var resources = interventions.Select(AgronomicInterventionResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}