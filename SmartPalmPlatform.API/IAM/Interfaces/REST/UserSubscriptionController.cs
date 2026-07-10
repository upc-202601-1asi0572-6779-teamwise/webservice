using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
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
[Route("api/v1/subscriptions")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("User Subscription endpoints")]
public class UserSubscriptionController(
    ISubscriptionCommandService subscriptionCommandService,
    ISubscriptionQueryService subscriptionQueryService,
    IPaymentQueryService paymentQueryService) : ControllerBase
{
    private int GetCurrentUserId()
    {
        var user = HttpContext.Items["User"] as User;
        return user!.Id;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get my subscription", OperationId = "GetMySubscription")]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscription found", typeof(SubscriptionResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Subscription not found")]
    public async Task<IActionResult> GetSubscription()
    {
        var query = new GetSubscriptionByUserIdQuery(GetCurrentUserId());
        var subscription = await subscriptionQueryService.Handle(query);
        if (subscription is null)
            return NotFound(new { message = "Subscription not found." });

        if (subscription.Status == Domain.Model.Enums.SubscriptionStatus.Active)
            return Ok(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(subscription));

        return Ok(new { status = subscription.Status.ToString(), planName = subscription.PlanName, amountDue = subscription.Price });
    }

    [HttpDelete]
    [SwaggerOperation(Summary = "Cancel my subscription", OperationId = "CancelMySubscription")]
    [SwaggerResponse(StatusCodes.Status200OK, "Subscription cancelled")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Subscription not found")]
    public async Task<IActionResult> CancelSubscription()
    {
        var command = new CancelSubscriptionCommand(GetCurrentUserId());
        var subscription = await subscriptionCommandService.Handle(command);
        return Ok(new { message = "Subscription cancelled.", status = subscription.Status.ToString() });
    }

    [HttpGet("payments")]
    [SwaggerOperation(Summary = "List my payments", OperationId = "ListMyPayments")]
    [SwaggerResponse(StatusCodes.Status200OK, "Payments found", typeof(IEnumerable<PaymentTransactionResource>))]
    public async Task<IActionResult> ListPayments()
    {
        var query = new GetPaymentsByUserIdQuery(GetCurrentUserId());
        var transactions = await paymentQueryService.Handle(query);
        var resources = transactions.Select(PaymentTransactionResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}