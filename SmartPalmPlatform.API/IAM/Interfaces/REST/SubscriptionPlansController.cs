using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Domain.Model.ValueObjects;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.IAM.Interfaces.REST;

[ApiController]
[Route("api/v1/subscriptions")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Subscription Plans")]
public class SubscriptionPlansController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("plans")]
    [SwaggerOperation(
        Summary = "List available subscription plans",
        Description = "Returns all available subscription plans with their descriptions and pricing.",
        OperationId = "ListSubscriptionPlans")]
    [SwaggerResponse(StatusCodes.Status200OK, "Plans found", typeof(IEnumerable<PlanResource>))]
    public IActionResult ListPlans()
    {
        var plans = SubscriptionPlanProvider.GetAll();
        var resources = plans.Select(PlanResourceFromSubscriptionPlanAssembler.ToResourceFromSubscriptionPlan);
        return Ok(resources);
    }
}
