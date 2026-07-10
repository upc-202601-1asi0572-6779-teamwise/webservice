using System.Text.RegularExpressions;
using SmartPalmPlatform.API.IAM.Domain.Model.Enums;
using SmartPalmPlatform.API.IAM.Domain.Services.DomainServices;

namespace SmartPalmPlatform.API.IAM.Application.Internal.DomainServices;

public class UserDomainService : IUserDomainService
{
    public bool IsValidId(int userId)
    {
        return userId > 0;
    }

    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }

    public bool IsValidUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        return username.Length >= 3 && username.Length <= 50;
    }

    public void ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.");
        if (username.Length < 3 || username.Length > 50)
            throw new ArgumentException("Username must be between 3 and 50 characters.");
    }

    public void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.");
        if (password.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters.");
    }

    public void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty.");
        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format.");
    }

    public UserRole ParseUserRole(string role)
    {
        if (!Enum.TryParse<UserRole>(role, true, out var parsed))
            throw new ArgumentException($"Invalid role '{role}'. Valid roles: Administrator, Agronomist, PalmGrower");
        return parsed;
    }
}
