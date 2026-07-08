using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartPalmPlatform.API.IAM.Domain.Model.Queries;
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
[SwaggerTag("Available User endpoints")]
public class UsersController(IUserQueryService userQueryService) : ControllerBase
{
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Get user by id",
        Description = "Get a user by its id",
        OperationId = "GetUserById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was found", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var getUserByIdQuery = new GetUserByIdQuery(id);
            var user = await userQueryService.Handle(getUserByIdQuery);

            if (user is null)
                return NotFound(new { message = "User not found." });

            var userResource = UserResourceFromEntityAssembler.ToResourceFromEntity(user);
            return Ok(userResource);
        }
        catch (Exception e)
        {
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
        try
        {
            var getAllUsersQuery = new GetAllUsersQuery();
            var users = await userQueryService.Handle(getAllUsersQuery);
            var userResources = users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity);
            return Ok(userResources);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }
}