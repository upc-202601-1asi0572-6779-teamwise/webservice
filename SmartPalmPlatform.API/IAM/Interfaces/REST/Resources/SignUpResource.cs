namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;

public record SignUpResource(string username, string password, string email, string fullName, string role);