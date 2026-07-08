namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;

public record AuthenticatedUserResource(int Id, string Username, string Role, string Token);