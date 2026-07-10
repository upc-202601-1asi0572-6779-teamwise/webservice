using SmartPalmPlatform.API.IAM.Domain.Model.Enums;

namespace SmartPalmPlatform.API.IAM.Domain.Services.DomainServices;

public interface IUserDomainService
{
    bool IsValidId(int userId);
    bool IsValidEmail(string email);
    bool IsValidUsername(string username);
    void ValidateUsername(string username);
    void ValidatePassword(string password);
    void ValidateEmail(string email);
    UserRole ParseUserRole(string role);
}