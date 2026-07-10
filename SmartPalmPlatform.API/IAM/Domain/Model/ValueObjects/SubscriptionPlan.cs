using SmartPalmPlatform.API.IAM.Domain.Model.Enums;

namespace SmartPalmPlatform.API.IAM.Domain.Model.ValueObjects;

public record SubscriptionPlan(
    PlanType Type,
    string Name,
    decimal Price,
    BillingCycle Cycle,
    int? MaxHectares,
    int? MaxSensors,
    int? MaxPlantationHistory
);

public static class SubscriptionPlanProvider
{
    public static SubscriptionPlan GetPlan(PlanType type) => type switch
    {
        PlanType.Seed => new SubscriptionPlan(
            PlanType.Seed, "Seed",
            149m, BillingCycle.Monthly, 50, 20, 3),

        PlanType.Harvest => new SubscriptionPlan(
            PlanType.Harvest, "Harvest",
            349m, BillingCycle.Monthly, null, null, null),

        PlanType.Custom => new SubscriptionPlan(
            PlanType.Custom, "Custom",
            0m, BillingCycle.Monthly, null, null, null),

        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public static IEnumerable<SubscriptionPlan> GetAll() =>
        Enum.GetValues<PlanType>().Select(GetPlan);
}
