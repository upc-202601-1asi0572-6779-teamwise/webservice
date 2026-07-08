namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;

public record UserResource(
    string username,
    string email,
    string fullName,
    string role,
    string status
);