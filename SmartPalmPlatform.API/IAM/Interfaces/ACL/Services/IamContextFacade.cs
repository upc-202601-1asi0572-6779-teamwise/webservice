using SmartPalmPlatform.API.IAM.Domain.Model.Commands;
using SmartPalmPlatform.API.IAM.Domain.Model.Enums;
using SmartPalmPlatform.API.IAM.Domain.Model.Queries;
using SmartPalmPlatform.API.IAM.Domain.Services;
using SmartPalmPlatform.API.IAM.Domain.Services.QueryServices;

namespace SmartPalmPlatform.API.IAM.Interfaces.ACL.Services;

public class IamContextFacade(
    IUserCommandService userCommandService,
    IUserQueryService userQueryService,
    ISubscriptionQueryService subscriptionQueryService
) : IIamContextFacade
{
    public async Task<int> CreateUser(string username, string password)
    {
        var signUpCommand = new SignUpCommand(username, password, string.Empty, string.Empty, "PalmGrower");
        await userCommandService.Handle(signUpCommand);
        var getUserByUsernameQuery = new GetUserByUsernameQuery(username);
        var result = await userQueryService.Handle(getUserByUsernameQuery);
        return result?.Id ?? 0;
    }

    public async Task<int> FetchUserIdByUsername(string username)
    {
        var getUserByUsernameQuery = new GetUserByUsernameQuery(username);
        var result = await userQueryService.Handle(getUserByUsernameQuery);
        return result?.Id ?? 0;
    }

    public async Task<string> FetchUsernameByUserId(int userId)
    {
        var getUserByIdQuery = new GetUserByIdQuery(userId);
        var result = await userQueryService.Handle(getUserByIdQuery);
        return result?.Username ?? string.Empty;
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        var getUserByUsernameQuery = new GetUserByUsernameQuery(username);
        var result = await userQueryService.Handle(getUserByUsernameQuery);
        return result is not null;
    }

    public async Task<bool> UserExistsByIdAsync(int userId)
    {
        var getUserByIdQuery = new GetUserByIdQuery(userId);
        var result = await userQueryService.Handle(getUserByIdQuery);
        return result is not null;
    }

    public async Task<bool> HasActiveSubscriptionAsync(int userId)
    {
        var query = new GetSubscriptionByUserIdQuery(userId);
        var subscription = await subscriptionQueryService.Handle(query);
        return subscription is not null
            && subscription.Status == SubscriptionStatus.Active
            && !subscription.IsExpired();
    }
}