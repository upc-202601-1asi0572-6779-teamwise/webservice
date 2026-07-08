namespace SmartPalmPlatform.API.IAM.Domain.Model.Commands;

public record ProcessPaymentCommand(int UserId, decimal Amount);
