using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Domain.Model.Enums;

namespace SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;

/**
 * This attribute is used to decorate controllers and actions that require authorization.
 * It checks if the user is logged in by checking if HttpContext.User is set.
 * If a user is not signed in, then it returns a 401-status code.
 * Optionally, set Roles (comma-separated) to also require the signed-in user to have
 * one of those roles — returns 403 if authenticated but not in an allowed role.
 */
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public string? Roles { get; set; }

    /**
     * <summary>
     *     This method is called when authorization is required.
     *     It checks if the user is logged in by checking if HttpContext.User is set.
     *     If a user is not signed in then it returns 401-status code. If Roles is set
     *     and the signed-in user's role isn't in it, it returns 403-status code.
     * </summary>
     * <param name="context">The authorization filter context</param>
     */
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();

        if (allowAnonymous)
        {
            Console.WriteLine(" Skipping authorization");
            return;
        }

        // verify if a user is signed in by checking if HttpContext.User is set
        var user = (User?)context.HttpContext.Items["User"];

        // if a user is not signed in, then return 401-status code
        if (user == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (string.IsNullOrWhiteSpace(Roles))
            return;

        var allowedRoles = Roles
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(role => Enum.Parse<Role>(role));

        if (!allowedRoles.Contains(user.Role))
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
    }
}