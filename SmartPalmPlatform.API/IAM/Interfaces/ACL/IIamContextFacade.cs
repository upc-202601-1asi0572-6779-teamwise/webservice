namespace SmartPalmPlatform.API.IAM.Interfaces.ACL;

public interface IIamContextFacade
{
    Task<int> CreateUser(string username, string password);
    Task<int> FetchUserIdByUsername(string username);
    Task<string> FetchUsernameByUserId(int userId);
    Task<bool> UserExistsAsync(string username);
    Task<bool> UserExistsByIdAsync(int userId);
    Task<bool> HasActiveSubscriptionAsync(int userId);
}