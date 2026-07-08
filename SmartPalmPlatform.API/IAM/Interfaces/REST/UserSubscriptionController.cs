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

namespace SmartPalmPlatform.API.IAM.Interfaces.REST;

[Authorize]
[ApiController]
[Route("api/v1/users/{userId}/subscription")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("User Subscription endpoints")]
public class UserSubscriptionController(
    ISubscriptionCommandService subscriptionCommandService,
    ISubscriptionQueryService subscriptionQueryService,
    IPaymentCommandService paymentCommandService,
    IPaymentQueryService paymentQueryService) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a subscription",
        Description = "Create a subscription for the specified user",
        OperationId = "CreateUserSubscription")]
    [SwaggerResponse(StatusCodes.Status201Created, "Subscription created", typeof(SubscriptionResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
    public async Task<IActionResult> CreateSubscription(int userId, [FromBody] CreateSubscriptionResource resource)
    {
        try
        {
            var command = CreateSubscriptionCommandFromResourceAssembler.ToCommandFromResource(userId, resource);
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
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get user subscription",
        Description = "Get the subscription for the specified user. Returns full details if active, summary if pending.",
        OperationId = "GetUserSubscription")]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscription found", typeof(SubscriptionResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Subscription not found")]
    public async Task<IActionResult> GetSubscription(int userId)
    {
        try
        {
            var query = new GetSubscriptionByUserIdQuery(userId);
            var subscription = await subscriptionQueryService.Handle(query);

            if (subscription is null)
                return NotFound(new { message = "Subscription not found for this user." });

            if (subscription.Status == Domain.Model.Enums.SubscriptionStatus.Active)
            {
                var resource = SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(subscription);
                return Ok(resource);
            }

            return Ok(new
            {
                status = subscription.Status.ToString(),
                planName = subscription.PlanName,
                amountDue = subscription.Price
            });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpDelete]
    [SwaggerOperation(
        Summary = "Cancel user subscription",
        Description = "Cancel the user's subscription. The subscription moves to history and the user can subscribe again.",
        OperationId = "CancelUserSubscription")]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscription cancelled", typeof(SubscriptionResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Subscription not found")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "Operation not allowed")]
    public async Task<IActionResult> CancelSubscription(int userId)
    {
        try
        {
            var command = new CancelSubscriptionCommand(userId);
            var subscription = await subscriptionCommandService.Handle(command);
            return Ok(new { message = "Subscription cancelled.", status = subscription.Status.ToString() });
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (InvalidOperationException e)
        {
            return Conflict(new { message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpGet("payments")]
    [SwaggerOperation(
        Summary = "List user payments",
        Description = "Get all payments made by the user.",
        OperationId = "ListUserPayments")]
    [SwaggerResponse(StatusCodes.Status200OK, "Payments found", typeof(IEnumerable<PaymentTransactionResource>))]
    public async Task<IActionResult> ListPayments(int userId)
    {
        try
        {
            var query = new GetPaymentsByUserIdQuery(userId);
            var transactions = await paymentQueryService.Handle(query);
            var resources = transactions
                .Select(PaymentTransactionResourceFromEntityAssembler.ToResourceFromEntity);
            return Ok(resources);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPost("payments")]
    [SwaggerOperation(
        Summary = "Process a payment",
        Description = "Process a payment for the user's subscription",
        OperationId = "ProcessUserSubscriptionPayment")]
    [SwaggerResponse(StatusCodes.Status201Created, "Payment processed", typeof(PaymentTransactionResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Subscription not found")]
    public async Task<IActionResult> ProcessPayment(int userId, [FromBody] ProcessPaymentResource resource)
    {
        try
        {
            var command = ProcessPaymentCommandFromResourceAssembler.ToCommandFromResource(userId, resource);
            var transaction = await paymentCommandService.Handle(command);
            var resourceOut = PaymentTransactionResourceFromEntityAssembler.ToResourceFromEntity(transaction);
            return StatusCode(StatusCodes.Status201Created, resourceOut);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (Exception e) when (e is ArgumentException)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }
}
