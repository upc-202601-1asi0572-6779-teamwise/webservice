namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;

public record PaymentTransactionResource(
    string planName,
    string periodStart,
    string periodEnd,
    decimal amount,
    string status,
    string processedAt
);
