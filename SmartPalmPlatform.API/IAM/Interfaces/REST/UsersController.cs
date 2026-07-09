using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Domain.Model.Queries;
using SmartPalmPlatform.API.IAM.Domain.Services;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.IAM.Interfaces.REST;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available User endpoints")]
public class UsersController(
    IUserQueryService userQueryService,
    IUserCommandService userCommandService
) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a user",
        Description = "Creates a new user account. Administrator-only: users cannot self-register.",
        OperationId = "CreateUser")]
    [SwaggerResponse(StatusCodes.Status201Created, "The user was created successfully", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid registration data")]
    public async Task<IActionResult> CreateUser([FromBody] SignUpResource resource)
    {
        try
        {
            var command = SignUpCommandFromResourceAssembler.ToCommandFromResource(resource);
            var user = await userCommandService.Handle(command);
            var userResource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);

            return Created(string.Empty, userResource);
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

    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get user by id",
        Description = "Get a user by its id",
        OperationId = "GetUserById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was found", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
    public async Task<IActionResult> GetUserById(int id)
    {
        Console.WriteLine($"[INFO] [IAM] [UsersController] GetUserById({id})");
        try
        {
            var getUserByIdQuery = new GetUserByIdQuery(id);
            var user = await userQueryService.Handle(getUserByIdQuery);

            if (user is null)
            {
                Console.WriteLine($"[WARN] [IAM] [UsersController] GetUserById({id}) — not found");
                return NotFound(new { message = "User not found." });
            }

            var userResource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);
            Console.WriteLine($"[INFO] [IAM] [UsersController] GetUserById({id}) — found '{user.Username}'");
            return Ok(userResource);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [IAM] [UsersController] GetUserById({id}): {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all users",
        Description = "Get all users",
        OperationId = "GetAllUsers")]
    [SwaggerResponse(StatusCodes.Status200OK, "The users were found", typeof(IEnumerable<UserResource>))]
    public async Task<IActionResult> GetAllUsers()
    {
        Console.WriteLine("[INFO] [IAM] [UsersController] GetAllUsers");
        try
        {
            var getAllUsersQuery = new GetAllUsersQuery();
            var users = await userQueryService.Handle(getAllUsersQuery);
            var userResources = users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity);
            Console.WriteLine($"[INFO] [IAM] [UsersController] GetAllUsers — returning {users.Count()} users");
            return Ok(userResources);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] [IAM] [UsersController] GetAllUsers: {e.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }
}