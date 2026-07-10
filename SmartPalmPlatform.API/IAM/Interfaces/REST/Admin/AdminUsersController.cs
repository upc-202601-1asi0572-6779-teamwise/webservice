using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Domain.Model.Queries;
using SmartPalmPlatform.API.IAM.Domain.Services;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Attributes;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Transform;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Admin;

[Authorize(Roles = "Administrator")]
[ApiController]
[Route("api/v1/admin/users")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Admin User management")]
public class AdminUsersController(
    IUserQueryService userQueryService,
    IUserCommandService userCommandService
) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create a new user",
        Description = "Creates a new user with the specified role. Admin only.",
        OperationId = "AdminCreateUser")]
    [SwaggerResponse(StatusCodes.Status201Created, "User created", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid data")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserResource resource)
    {
        try
        {
            var command = CreateUserCommandFromResourceAssembler.ToCommandFromResource(resource);
            var user = await userCommandService.Handle(command);
            var response = UserResourceFromEntityAssembler.ToResourceFromEntity(user);
            return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, response);
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "List all users",
        Description = "Returns all registered users.",
        OperationId = "AdminListUsers")]
    [SwaggerResponse(StatusCodes.Status200OK, "Users found", typeof(IEnumerable<UserResource>))]
    public async Task<IActionResult> ListUsers()
    {
        var query = new GetAllUsersQuery();
        var users = await userQueryService.Handle(query);
        var resources = users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("{userId:int}")]
    [SwaggerOperation(
        Summary = "Get a user by ID",
        Description = "Returns a specific user.",
        OperationId = "AdminGetUserById")]
    [SwaggerResponse(StatusCodes.Status200OK, "User found", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var query = new GetUserByIdQuery(userId);
        var user = await userQueryService.Handle(query);
        if (user is null)
            return NotFound(new { message = "User not found." });
        var resource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);
        return Ok(resource);
    }
}