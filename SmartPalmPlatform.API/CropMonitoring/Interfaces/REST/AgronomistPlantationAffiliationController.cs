using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.CropMonitoring.Domain.Commands;
using SmartPalmPlatform.API.CropMonitoring.Domain.Queries;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Transform;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST;

[Authorize]
[RequireActiveSubscription]
[ApiController]
[Route("api/v1/agronomists")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Agronomist-plantation affiliation endpoints")]
public class AgronomistPlantationAffiliationController(
    IAgronomistPlantationAffiliationCommandService commandService,
    IAgronomistPlantationAffiliationQueryService queryService
) : ControllerBase
{
    [HttpGet("{agronomistId:int}/plantation-affiliations")]
    [SwaggerOperation(
        Summary = "Get agronomist plantation affiliations",
        Description = "Returns all plantation affiliations for an agronomist.",
        OperationId = "GetAgronomistPlantationAffiliations")]
    [SwaggerResponse(StatusCodes.Status200OK, "Affiliations found",
        typeof(IEnumerable<AgronomistPlantationAffiliationResource>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Unauthorized access")]
    public async Task<IActionResult> GetAgronomistAffiliations(int agronomistId)
    {
        var query = new GetAgronomistPlantationAffiliationsQuery(agronomistId);
        var result = await queryService.Handle(query);
        return Ok(AgronomistPlantationAffiliationAssembler.ToResourceListFromEntityList(result));
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("plantation-affiliations")]
    [SwaggerOperation(
        Summary = "Create agronomist-plantation affiliation",
        Description = "Creates a new affiliation between an agronomist and a plantation. Admin only.",
        OperationId = "CreateAgronomistPlantationAffiliation")]
    [SwaggerResponse(StatusCodes.Status201Created, "Affiliation created",
        typeof(AgronomistPlantationAffiliationResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid payload")]
    public async Task<IActionResult> CreateAffiliation(
        [FromBody] CreateAgronomistPlantationAffiliationResource resource)
    {
        try
        {
            var command = new CreateAgronomistPlantationAffiliationCommand(
                resource.AgronomistId, resource.PlantationId);
            var result = await commandService.Handle(command);
            var response = AgronomistPlantationAffiliationAssembler.ToResourceFromEntity(result);
            return CreatedAtAction(
                nameof(PlantationsController.GetPlantationAgronomistAffiliations),
                nameof(PlantationsController).Replace("Controller", ""),
                new { plantationId = result.PlantationId }, response);
        }
        catch (KeyNotFoundException e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete("{agronomistId:int}/plantation-affiliations/{plantationId:int}")]
    [SwaggerOperation(
        Summary = "Detach agronomist-plantation affiliation",
        Description = "Detaches an agronomist from a plantation. Admin only.",
        OperationId = "DetachAgronomistPlantationAffiliation")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Affiliation detached")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Affiliation not found")]
    public async Task<IActionResult> DetachAffiliation(int agronomistId, int plantationId)
    {
        try
        {
            var command = new DetachAgronomistPlantationAffiliationCommand(agronomistId, plantationId);
            await commandService.Handle(command);
            return NoContent();
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
    }
}