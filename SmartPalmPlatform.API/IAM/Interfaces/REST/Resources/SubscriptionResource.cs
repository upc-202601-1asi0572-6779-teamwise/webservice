namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;

public record SubscriptionResource(
    string planType,
    string planName,
    decimal price,
    string status,
    string startDate,
    string endDate,
    string billingCycle,
    string createdAt
);
