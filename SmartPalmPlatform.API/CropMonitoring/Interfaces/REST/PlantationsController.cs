using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Queries;
using SmartPalmPlatform.API.CropMonitoring.Domain.Queries;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services.CommandServices;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services.QueryServices;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Transform;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST;

[Authorize(Roles = "Administrator,PalmGrower")]
[RequireActiveSubscription]
[ApiController]
[Route("api/v1/plantations")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Plantation management endpoints")]
public class PlantationsController(
    IPlantationCommandService plantationCommandService,
    IPlantationQueryService plantationQueryService,
    IAgronomistPlantationAffiliationQueryService affiliationQueryService
) : ControllerBase
{
    private int GetCurrentUserId()
    {
        var user = HttpContext.Items["User"] as dynamic;
        return user!.Id;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new plantation",
        Description = "Creates a plantation in Installing status.",
        OperationId = "CreatePlantation")]
    [SwaggerResponse(StatusCodes.Status201Created, "Plantation created", typeof(PlantationResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid data")]
    public async Task<IActionResult> CreatePlantation([FromBody] CreatePlantationResource resource)
    {
        var command = CreatePlantationCommandFromResourceAssembler.ToCommandFromResource(GetCurrentUserId(), resource);
        var plantation = await plantationCommandService.Handle(command);
        var output = PlantationResourceFromEntityAssembler.ToResourceFromEntity(plantation);
        return StatusCode(StatusCodes.Status201Created, output);
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get my plantations",
        Description = "Returns all plantations for the authenticated user.",
        OperationId = "GetMyPlantations")]
    [SwaggerResponse(StatusCodes.Status200OK, "Plantations found", typeof(IEnumerable<PlantationResource>))]
    public async Task<IActionResult> GetMyPlantations()
    {
        var query = new GetPlantationsByUserIdQuery(GetCurrentUserId());
        var plantations = await plantationQueryService.Handle(query);
        var resources = plantations.Select(PlantationResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("{plantationId:int}")]
    [SwaggerOperation(
        Summary = "Get plantation by ID",
        Description = "Returns the plantation detail including all sectors.",
        OperationId = "GetPlantationById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Plantation found", typeof(PlantationDetailResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Plantation not found")]
    public async Task<IActionResult> GetPlantationById(int plantationId)
    {
        var query = new GetPlantationByIdQuery(plantationId);
        var plantation = await plantationQueryService.Handle(query);
        if (plantation is null)
            return NotFound(new { message = "Plantation not found." });
        var output = PlantationResourceFromEntityAssembler.ToDetailResourceFromEntity(plantation);
        return Ok(output);
    }

    [HttpPatch("{plantationId:int}")]
    [SwaggerOperation(
        Summary = "Update plantation details",
        Description = "Updates the plantation name, location, and hectares.",
        OperationId = "UpdatePlantation")]
    [SwaggerResponse(StatusCodes.Status200OK, "Plantation updated", typeof(PlantationResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Plantation not found")]
    public async Task<IActionResult> UpdatePlantation(int plantationId, [FromBody] UpdatePlantationResource resource)
    {
        var command = UpdatePlantationCommandFromResourceAssembler.ToCommandFromResource(plantationId, resource);
        var plantation = await plantationCommandService.Handle(command);
        var output = PlantationResourceFromEntityAssembler.ToResourceFromEntity(plantation);
        return Ok(output);
    }

    [HttpGet("{plantationId:int}/agronomist-affiliations")]
    [SwaggerOperation(
        Summary = "Get agronomist affiliations for a plantation",
        Description = "Returns all agronomist affiliations for a plantation.",
        OperationId = "GetPlantationAgronomistAffiliations")]
    [SwaggerResponse(StatusCodes.Status200OK, "Affiliations found",
        typeof(IEnumerable<AgronomistPlantationAffiliationResource>))]
    public async Task<IActionResult> GetPlantationAgronomistAffiliations(int plantationId)
    {
        var query = new GetPlantationAgronomistAffiliationsQuery(plantationId);
        var result = await affiliationQueryService.Handle(query);
        return Ok(AgronomistPlantationAffiliationAssembler.ToResourceListFromEntityList(result));
    }
}