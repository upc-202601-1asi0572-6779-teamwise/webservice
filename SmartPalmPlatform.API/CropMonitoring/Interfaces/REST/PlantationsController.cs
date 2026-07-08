using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Queries;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services.CommandServices;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services.QueryServices;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Transform;
using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST;

[Authorize]
[ApiController]
[Route("api/v1/plantations")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Plantation management endpoints")]
public class PlantationsController(
    IPlantationCommandService plantationCommandService,
    IPlantationQueryService plantationQueryService
) : ControllerBase
{
    private int GetCurrentUserId()
    {
        var user = HttpContext.Items["User"] as User;
        return user?.Id ?? 0;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new plantation",
        Description = "Creates a plantation in Installing status. An installation plan is generated automatically.",
        OperationId = "CreatePlantation"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Plantation created", typeof(PlantationResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid data")]
    public async Task<IActionResult> CreatePlantation([FromBody] CreatePlantationResource resource)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "User not authenticated." });

            var command = CreatePlantationCommandFromResourceAssembler.ToCommandFromResource(
                userId,
                resource
            );
            var plantation = await plantationCommandService.Handle(command);
            var output = PlantationResourceFromEntityAssembler.ToResourceFromEntity(plantation);
            return StatusCode(StatusCodes.Status201Created, output);
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
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

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get user's plantations",
        Description = "Returns all plantations for the authenticated user.",
        OperationId = "GetMyPlantations"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Plantations found", typeof(IEnumerable<PlantationResource>))]
    public async Task<IActionResult> GetMyPlantations()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized(new { message = "User not authenticated." });

            var query = new GetPlantationsByUserIdQuery(userId);
            var plantations = await plantationQueryService.Handle(query);
            var resources = plantations.Select(
                PlantationResourceFromEntityAssembler.ToResourceFromEntity
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

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get plantation by ID",
        Description = "Returns the plantation detail including all sectors.",
        OperationId = "GetPlantationById"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Plantation found", typeof(PlantationDetailResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Plantation not found")]
    public async Task<IActionResult> GetPlantationById(int id)
    {
        try
        {
            var query = new GetPlantationByIdQuery(id);
            var plantation = await plantationQueryService.Handle(query);
            if (plantation is null)
                return NotFound(new { message = "Plantation not found." });

            var output = PlantationResourceFromEntityAssembler.ToDetailResourceFromEntity(plantation);
            return Ok(output);
        }
        catch (Exception e)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = e.Message }
            );
        }
    }

    [HttpPatch("{id}")]
    [SwaggerOperation(
        Summary = "Update plantation details",
        Description = "Updates the plantation name, location, and hectares.",
        OperationId = "UpdatePlantation"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Plantation updated", typeof(PlantationResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Plantation not found")]
    public async Task<IActionResult> UpdatePlantation(
        int id,
        [FromBody] UpdatePlantationResource resource
    )
    {
        try
        {
            var command = UpdatePlantationCommandFromResourceAssembler.ToCommandFromResource(
                id,
                resource
            );
            var plantation = await plantationCommandService.Handle(command);
            var output = PlantationResourceFromEntityAssembler.ToResourceFromEntity(plantation);
            return Ok(output);
        }
        catch (Exception e) when (e is KeyNotFoundException)
        {
            return NotFound(new { message = e.Message });
        }
        catch (Exception e) when (e is InvalidOperationException)
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
}
