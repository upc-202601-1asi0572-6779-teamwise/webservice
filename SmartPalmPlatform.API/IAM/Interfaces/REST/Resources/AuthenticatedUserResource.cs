namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;

public record AuthenticatedUserResource(
    int UserId,
    string Username,
    string Email,
    string FullName,
    string Role,
    string Token
);