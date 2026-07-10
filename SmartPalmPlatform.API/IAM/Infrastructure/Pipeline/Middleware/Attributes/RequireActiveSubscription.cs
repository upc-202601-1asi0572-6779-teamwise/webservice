using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Domain.Model.Enums;

namespace SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequireActiveSubscription : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        if (endpoint is not null
            && endpoint.Metadata.Any(m => m.GetType() == typeof(AllowAnonymousAttribute)))
        {
            return;
        }

        var user = context.HttpContext.Items["User"] as User;

        if (user is null)
        {
            context.Result = new UnauthorizedObjectResult(new { message = "User not authenticated." });
            return;
        }

        if (user.Role == UserRole.Administrator)
            return;

        var facade = context.HttpContext.RequestServices.GetRequiredService<Interfaces.ACL.IIamContextFacade>();
        var hasActive = await facade.HasActiveSubscriptionAsync(user.Id);

        if (!hasActive)
        {
            context.Result = new ForbidResult();
        }
    }
}