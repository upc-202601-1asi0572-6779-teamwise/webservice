using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Domain.Model.Commands;
using SmartPalmPlatform.API.IAM.Domain.Model.Queries;
using SmartPalmPlatform.API.IAM.Domain.Services.CommandServices;
using SmartPalmPlatform.API.IAM.Domain.Services.QueryServices;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Admin;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/v1/admin/subscriptions")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Admin Subscription management")]
public class AdminSubscriptionController(
    ISubscriptionCommandService subscriptionCommandService,
    ISubscriptionQueryService subscriptionQueryService,
    IPaymentCommandService paymentCommandService
) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a subscription for a user",
        Description = "Admin creates a subscription for a specific user.",
        OperationId = "AdminCreateSubscription")]
    [SwaggerResponse(StatusCodes.Status201Created, "Subscription created", typeof(SubscriptionResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionResource resource)
    {
        try
        {
            var command = CreateSubscriptionCommandFromResourceAssembler.ToCommandFromResource(
                resource.UserId, resource);
            var subscription = await subscriptionCommandService.Handle(command);
            var subscriptionResource = SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(subscription);
            return StatusCode(StatusCodes.Status201Created, subscriptionResource);
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "List all subscriptions",
        Description = "Returns all subscriptions in the platform.",
        OperationId = "AdminListSubscriptions")]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscriptions found", typeof(IEnumerable<SubscriptionResource>))]
    public async Task<IActionResult> ListSubscriptions()
    {
        var query = new GetAllSubscriptionsQuery();
        var subscriptions = await subscriptionQueryService.Handle(query);
        var resources = subscriptions.Select(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("{subscriptionId:int}")]
    [SwaggerOperation(
        Summary = "Get a subscription by ID",
        Description = "Returns a specific subscription.",
        OperationId = "AdminGetSubscriptionById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscription found", typeof(SubscriptionResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Subscription not found")]
    public async Task<IActionResult> GetSubscriptionById(int subscriptionId)
    {
        var query = new GetSubscriptionByIdQuery(subscriptionId);
        var subscription = await subscriptionQueryService.Handle(query);
        if (subscription is null)
            return NotFound(new { message = "Subscription not found." });
        var resource = SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(subscription);
        return Ok(resource);
    }

    [HttpPost("users/{userId:int}/payments")]
    [SwaggerOperation(
        Summary = "Process a payment for a user's subscription",
        Description = "Admin processes a payment for a specific user.",
        OperationId = "AdminProcessPayment")]
    [SwaggerResponse(StatusCodes.Status201Created, "Payment processed", typeof(PaymentTransactionResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Subscription not found")]
    public async Task<IActionResult> ProcessPayment(int userId, [FromBody] ProcessPaymentResource resource)
    {
        try
        {
            var command = ProcessPaymentCommandFromResourceAssembler.ToCommandFromResource(userId, resource);
            var transaction = await paymentCommandService.Handle(command);
            var output = PaymentTransactionResourceFromEntityAssembler.ToResourceFromEntity(transaction);
            return StatusCode(StatusCodes.Status201Created, output);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (Exception e) when (e is ArgumentException)
        {
            return BadRequest(new { message = e.Message });
        }
    }
}