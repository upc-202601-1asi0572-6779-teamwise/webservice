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
        Console.WriteLine($"[INFO] [IAM] [IamContextFacade] CreateUser('{username}')");
        var command = new CreateUserCommand(username, password, string.Empty, string.Empty, "PalmGrower");
        await userCommandService.Handle(command);
        var getUserByUsernameQuery = new GetUserByUsernameQuery(username);
        var result = await userQueryService.Handle(getUserByUsernameQuery);
        Console.WriteLine($"[INFO] [IAM] [IamContextFacade] CreateUser('{username}') → userId={result?.Id}");
        return result?.Id ?? 0;
    }

    public async Task<int> FetchUserIdByUsername(string username)
    {
        Console.WriteLine($"[INFO] [IAM] [IamContextFacade] FetchUserIdByUsername('{username}')");
        var getUserByUsernameQuery = new GetUserByUsernameQuery(username);
        var result = await userQueryService.Handle(getUserByUsernameQuery);
        Console.WriteLine($"[INFO] [IAM] [IamContextFacade] FetchUserIdByUsername('{username}') → {result?.Id}");
        return result?.Id ?? 0;
    }

    public async Task<string> FetchUsernameByUserId(int userId)
    {
        Console.WriteLine($"[INFO] [IAM] [IamContextFacade] FetchUsernameByUserId({userId})");
        var getUserByIdQuery = new GetUserByIdQuery(userId);
        var result = await userQueryService.Handle(getUserByIdQuery);
        Console.WriteLine($"[INFO] [IAM] [IamContextFacade] FetchUsernameByUserId({userId}) → '{result?.Username}'");
        return result?.Username ?? string.Empty;
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        Console.WriteLine($"[INFO] [IAM] [IamContextFacade] UserExistsAsync('{username}')");
        var getUserByUsernameQuery = new GetUserByUsernameQuery(username);
        var result = await userQueryService.Handle(getUserByUsernameQuery);
        Console.WriteLine($"[INFO] [IAM] [IamContextFacade] UserExistsAsync('{username}') → {result is not null}");
        return result is not null;
    }

    public async Task<bool> UserExistsByIdAsync(int userId)
    {
        Console.WriteLine($"[INFO] [IAM] [IamContextFacade] UserExistsByIdAsync({userId})");
        var getUserByIdQuery = new GetUserByIdQuery(userId);
        var result = await userQueryService.Handle(getUserByIdQuery);
        Console.WriteLine($"[INFO] [IAM] [IamContextFacade] UserExistsByIdAsync({userId}) → {result is not null}");
        return result is not null;
    }

    public async Task<bool> HasActiveSubscriptionAsync(int userId)
    {
        Console.WriteLine($"[INFO] [IAM] [IamContextFacade] HasActiveSubscriptionAsync({userId})");

        var userQuery = await userQueryService.Handle(new GetUserByIdQuery(userId));
        if (userQuery is not null && userQuery.Role == UserRole.Administrator)
        {
            Console.WriteLine($"[INFO] [IAM] [IamContextFacade] HasActiveSubscriptionAsync({userId}) → true (Administrator exempt)");
            return true;
        }

        var query = new GetSubscriptionByUserIdQuery(userId);
        var subscription = await subscriptionQueryService.Handle(query);
        var active = subscription is not null
            && subscription.Status == SubscriptionStatus.Active
            && !subscription.IsExpired();
        Console.WriteLine($"[INFO] [IAM] [IamContextFacade] HasActiveSubscriptionAsync({userId}) → {active}");
        return active;
    }
}