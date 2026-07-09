using System.Text.Json.Serialization;
using SmartPalmPlatform.API.IAM.Domain.Model.Enums;

namespace SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;

public partial class User(string username, string passwordHash, string email, string fullName, UserRole role = UserRole.PalmGrower)
{
    public User() : this(string.Empty, string.Empty, string.Empty, string.Empty)
    {
    }

    public int Id { get; }
    public string Username { get; private set; } = username;

    [JsonIgnore]
    public string PasswordHash { get; private set; } = passwordHash;

    public string Email { get; private set; } = email;
    public string FullName { get; private set; } = fullName;
    public UserRole Role { get; private set; } = role;
    public UserStatus Status { get; private set; } = UserStatus.Active;
    public int? SubscriptionId { get; private set; }

    public User UpdateUsername(string username)
    {
        Username = username;
        return this;
    }

    public User UpdatePasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        return this;
    }

    public User UpdateProfile(string fullName, string email)
    {
        FullName = fullName;
        Email = email;
        return this;
    }

    public User UpdateSubscription(int? subscriptionId)
    {
        SubscriptionId = subscriptionId;
        return this;
    }

    public void ActivateAccess()
    {
        Status = UserStatus.Active;
    }

    public void RevokeAccess()
    {
        Status = UserStatus.Inactive;
    }
}