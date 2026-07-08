using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Commands;
using SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services.CommandServices;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Transform;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/v1/plantations/{plantationId}/sectors")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Plantation sector management (admin)")]
public class PlantationSectorsController(
    IPlantationCommandService plantationCommandService,
    ISectorRepository sectorRepository
) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Assign an IoT device as a sector",
        Description = "Assigns an existing IoT device to a plantation as a monitoring sector.",
        OperationId = "AssignSector"
    )]
    [SwaggerResponse(StatusCodes.Status201Created, "Sector assigned")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Plantation not found")]
    public async Task<IActionResult> AssignSector(
        int plantationId,
        [FromBody] AssignSectorResource resource
    )
    {
        try
        {
            var command = AssignSectorCommandFromResourceAssembler.ToCommandFromResource(
                plantationId,
                resource
            );
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
        catch (Exception e)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = e.Message }
            );
        }
    }

    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Get sectors by plantation",
        Description = "Returns all sectors for a plantation.",
        OperationId = "GetPlantationSectors"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Sectors found", typeof(IEnumerable<SectorResource>))]
    public async Task<IActionResult> GetSectors(int plantationId)
    {
        try
        {
            var sectors = await sectorRepository.FindByPlantationIdAsync(plantationId);
            var resources = sectors.Select(
                SectorResourceFromEntityAssembler.ToResourceFromEntity
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

    [HttpDelete("{sectorId}")]
    [SwaggerOperation(
        Summary = "Remove a sector from plantation",
        Description = "Removes a sector. The IoT device can be reassigned to another plantation.",
        OperationId = "RemoveSector"
    )]
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
        catch (Exception e)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = e.Message }
            );
        }
    }
}
