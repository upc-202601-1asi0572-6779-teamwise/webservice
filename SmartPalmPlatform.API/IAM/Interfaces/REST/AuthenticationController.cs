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
        try
        {
            var signInCommand = SignInCommandFromResourceAssembler.ToCommandFromResource(signInResource);
            var authenticatedUser = await userCommandService.Handle(signInCommand);
            var resource = AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(
                authenticatedUser.user, authenticatedUser.token);
            return Ok(resource);
        }
        catch (Exception e)
        {
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
        try
        {
            var signUpCommand = SignUpCommandFromResourceAssembler.ToCommandFromResource(signUpResource);
            await userCommandService.Handle(signUpCommand);
            return Ok(new { message = "User created successfully" });
        }
        catch (Exception e) when (e is ArgumentException)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }
}
