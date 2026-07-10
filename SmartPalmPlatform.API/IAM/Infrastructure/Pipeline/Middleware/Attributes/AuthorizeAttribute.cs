using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;

namespace SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public string? Roles { get; set; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata
            .OfType<AllowAnonymousAttribute>().Any();

        if (allowAnonymous)
            return;

        var user = (User?)context.HttpContext.Items["User"];

        if (user == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!string.IsNullOrEmpty(Roles))
        {
            var requiredRoles = Roles.Split(',').Select(r => r.Trim()).ToList();
            var userRoleName = Enum.GetName(user.Role);

            if (userRoleName == null || !requiredRoles.Contains(userRoleName))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }
    }
}
