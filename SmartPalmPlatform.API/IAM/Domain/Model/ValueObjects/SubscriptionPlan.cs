using SmartPalmPlatform.API.IAM.Domain.Model.Enums;

namespace SmartPalmPlatform.API.IAM.Domain.Model.ValueObjects;

public record SubscriptionPlan(
    PlanType Type,
    string Name,
    string Description,
    decimal Price,
    BillingCycle Cycle,
    int? MaxHectares,
    int? MaxSensors
);

public static class SubscriptionPlanProvider
{
    public static SubscriptionPlan GetPlan(PlanType type) => type switch
    {
        PlanType.Seed => new SubscriptionPlan(
            PlanType.Seed, "Seed",
            "Basic plan for small producers. Includes monitoring of up to 50 hectares with 20 sensors.",
            149m, BillingCycle.Monthly, 50, 20),

        PlanType.Harvest => new SubscriptionPlan(
            PlanType.Harvest, "Harvest",
            "Intermediate plan for medium producers. Unlimited hectares and sensors, ideal for growing operations.",
            349m, BillingCycle.Monthly, null, null),

        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public static IEnumerable<SubscriptionPlan> GetAll() =>
        Enum.GetValues<PlanType>().Select(GetPlan);
}
