using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Commands;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Queries;
using SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services.CommandServices;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services.QueryServices;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Transform;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Admin;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/v1/admin/plantations")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Admin Plantation management")]
public class AdminPlantationsController(
    IPlantationQueryService plantationQueryService,
    IPlantationCommandService plantationCommandService
) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(
        Summary = "List all plantations",
        Description = "Returns all plantations in the platform.",
        OperationId = "AdminListPlantations")]
    [SwaggerResponse(StatusCodes.Status200OK, "Plantations found", typeof(IEnumerable<PlantationResource>))]
    public async Task<IActionResult> ListPlantations()
    {
        var query = new GetAllPlantationsQuery();
        var plantations = await plantationQueryService.Handle(query);
        var resources = plantations.Select(PlantationResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("{plantationId:int}")]
    [SwaggerOperation(
        Summary = "Get a plantation by ID",
        Description = "Returns a specific plantation with sectors.",
        OperationId = "AdminGetPlantationById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Plantation found", typeof(PlantationDetailResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Plantation not found")]
    public async Task<IActionResult> GetPlantationById(int plantationId)
    {
        var query = new GetPlantationByIdQuery(plantationId);
        var plantation = await plantationQueryService.Handle(query);
        if (plantation is null)
            return NotFound(new { message = "Plantation not found." });
        var resource = PlantationResourceFromEntityAssembler.ToDetailResourceFromEntity(plantation);
        return Ok(resource);
    }

    [HttpPost("{plantationId:int}/sectors")]
    [SwaggerOperation(
        Summary = "Assign a sector to a plantation",
        Description = "Assigns an IoT device as a monitoring sector.",
        OperationId = "AdminAssignSector")]
    [SwaggerResponse(StatusCodes.Status201Created, "Sector assigned")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Plantation not found")]
    public async Task<IActionResult> AssignSector(int plantationId, [FromBody] AssignSectorResource resource)
    {
        try
        {
            var command = AssignSectorCommandFromResourceAssembler.ToCommandFromResource(plantationId, resource);
            await plantationCommandService.Handle(command);
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception e) when (e is KeyNotFoundException)
        {
            return NotFound(new { message = e.Message });
        }
        catch (Exception e) when (e is InvalidOperationException or ArgumentException)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpDelete("{plantationId:int}/sectors/{sectorId:int}")]
    [SwaggerOperation(
        Summary = "Remove a sector from a plantation",
        Description = "Removes a sector. The IoT device can be reassigned.",
        OperationId = "AdminRemoveSector")]
    [SwaggerResponse(StatusCodes.Status200OK, "Sector removed")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Sector not found")]
    public async Task<IActionResult> RemoveSector(int plantationId, int sectorId)
    {
        try
        {
            var command = new RemoveSectorCommand(plantationId, sectorId);
            await plantationCommandService.Handle(command);
            return Ok(new { message = "Sector removed." });
        }
        catch (Exception e) when (e is KeyNotFoundException)
        {
            return NotFound(new { message = e.Message });
        }
    }
}