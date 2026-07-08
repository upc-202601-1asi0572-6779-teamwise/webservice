namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;

public record SubscriptionResource(
    string planType,
    string planName,
    decimal price,
    int? maxHectares,
    int? maxSensors,
    string status,
    string startDate,
    string endDate,
    string billingCycle,
    string createdAt
);
