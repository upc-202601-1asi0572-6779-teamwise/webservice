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

    [HttpPost("sign-up")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Sign up",
        Description = "Sign up a new user",
        OperationId = "SignUp")]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was created successfully")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid registration data")]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource signUpResource)
    {
        Console.WriteLine($"[INFO] [IAM] [AuthenticationController] SignUp attempt for username '{signUpResource.username}'");
        try
        {
            var signUpCommand = SignUpCommandFromResourceAssembler.ToCommandFromResource(signUpResource);
            await userCommandService.Handle(signUpCommand);
            Console.WriteLine($"[INFO] [IAM] [AuthenticationController] SignUp successful for user '{signUpResource.username}'");
            return Ok(new { message = "User created successfully" });
        }
        catch (Exception e) when (e is ArgumentException)
        {
            Console.WriteLine($"[WARN] [IAM] [AuthenticationController] SignUp validation failed for '{signUpResource.username}': {e.Message}");
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [IAM] [AuthenticationController] SignUp error for '{signUpResource.username}': {e.Message}");
            return BadRequest(new { message = e.Message });
        }
    }
}
