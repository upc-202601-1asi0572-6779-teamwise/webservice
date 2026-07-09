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
[Route("api/v1/field/interventions")]
[Produces(MediaTypeNames.Application.Json)]
[ApiController]
[SwaggerTag("Field Technical Management - Intervention endpoints")]
public class AgronomicInterventionController(
    IAgronomicInterventionCommandService commandService,
    IAgronomicInterventionQueryService queryService
) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Register a new agronomic intervention",
        Description = "Registers a new agronomic intervention for a plantation or zone.",
        OperationId = "RegisterIntervention")]
    [SwaggerResponse(
        StatusCodes.Status201Created,
        "Intervention created",
        typeof(AgronomicInterventionResource)
    )]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
    public async Task<IActionResult> RegisterIntervention(
        [FromBody] RegisterInterventionResource resource
    )
    {
        Console.WriteLine($"[INFO] [BC] [AgronomicIntervention] RegisterIntervention called");
        try
        {
            var command = RegisterCommandFromResourceAssembler.ToCommandFromResource(resource);
            var intervention = await commandService.Handle(command);
            var response = AgronomicInterventionResourceFromEntityAssembler.ToResourceFromEntity(
                intervention
            );
            Console.WriteLine($"[INFO] [BC] [AgronomicIntervention] Intervention registered with id: {intervention.Id}");
            return Created($"/api/v1/field/interventions/{intervention.Id}", response);
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
        {
            Console.WriteLine($"[WARN] [BC] [AgronomicIntervention] Validation failed registering intervention - {e.Message}");
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [BC] [AgronomicIntervention] Error registering intervention - {e.Message}");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = e.Message }
            );
        }
    }

    [HttpGet("{interventionId:int}")]
    [SwaggerOperation(
        Summary = "Get intervention by ID",
        Description = "Returns the detail of a specific intervention with its complete traceability.",
        OperationId = "GetAgronomicInterventionById")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "The intervention was found",
        typeof(AgronomicInterventionResource)
    )]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The intervention was not found")]
    public async Task<IActionResult> GetAgronomicInterventionById([FromRoute] int interventionId)
    {
        Console.WriteLine($"[INFO] [BC] [AgronomicIntervention] GetAgronomicInterventionById called with interventionId: {interventionId}");
        try
        {
            var query = new GetAgronomicInterventionByIdQuery(interventionId);
            var intervention = await queryService.Handle(query);
            if (intervention is null)
            {
                Console.WriteLine($"[WARN] [BC] [AgronomicIntervention] Intervention not found with id: {interventionId}");
                return NotFound(new { message = "Intervention not found." });
            }
            var resource = AgronomicInterventionResourceFromEntityAssembler.ToResourceFromEntity(
                intervention
            );
            Console.WriteLine($"[INFO] [BC] [AgronomicIntervention] Intervention found with id: {interventionId}");
            return Ok(resource);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [BC] [AgronomicIntervention] Error getting intervention id: {interventionId} - {e.Message}");
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = e.Message }
            );
        }
    }
}
