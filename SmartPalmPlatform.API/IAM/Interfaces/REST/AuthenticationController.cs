using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Domain.Services;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.IAM.Interfaces.REST;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Authentication endpoints")]
public class AuthenticationController(IUserCommandService userCommandService) : ControllerBase
{
    [HttpPost("sign-in")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Sign in",
        Description = "Sign in a user",
        OperationId = "SignIn")]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was authenticated", typeof(AuthenticatedUserResource))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Invalid credentials")]
    public async Task<IActionResult> SignIn([FromBody] SignInResource signInResource)
    {
        Console.WriteLine($"[INFO] [IAM] [AuthenticationController] SignIn attempt for user '{signInResource.Username}'");
        try
        {
            var signInCommand = SignInCommandFromResourceAssembler.ToCommandFromResource(signInResource);
            var authenticatedUser = await userCommandService.Handle(signInCommand);
            var resource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(
                authenticatedUser.user, authenticatedUser.token);
            Console.WriteLine($"[INFO] [IAM] [AuthenticationController] SignIn successful for user '{signInResource.Username}' (userId={authenticatedUser.user.Id})");
            return Ok(resource);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[WARN] [IAM] [AuthenticationController] SignIn failed for user '{signInResource.Username}': {e.Message}");
            return Unauthorized(new { message = e.Message });
        }
    }
}
