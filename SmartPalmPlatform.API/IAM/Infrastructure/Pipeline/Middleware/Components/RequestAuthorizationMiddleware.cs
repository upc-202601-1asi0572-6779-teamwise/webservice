using SmartPalmPlatform.API.IAM.Application.Internal.OutboundServices;
using SmartPalmPlatform.API.IAM.Domain.Model.Queries;
using SmartPalmPlatform.API.IAM.Domain.Services;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;

namespace SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Components;

public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IUserQueryService userQueryService,
        ITokenService tokenService)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";
        if (path.Contains("/authentication/") || path.Contains("/swagger"))
        {
            await next(context);
            return;
        }

        var endpoint = context.Request.HttpContext.GetEndpoint();
        if (endpoint != null)
        {
            var allowAnonymous = endpoint.Metadata
                .Any(m => m.GetType() == typeof(AllowAnonymousAttribute));

            if (allowAnonymous)
            {
                await next(context);
                return;
            }
        }

        try
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                await next(context);
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var userId = await tokenService.ValidateToken(token);
            if (userId == null)
            {
                await next(context);
                return;
            }

            var getUserByIdQuery = new GetUserByIdQuery(userId.Value);
            var user = await userQueryService.Handle(getUserByIdQuery);

            if (user != null)
            {
                context.Items["User"] = user;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARN] [IAM] [RequestAuthorizationMiddleware] Error validating token: {ex.Message}");
        }

        await next(context);
    }
}
