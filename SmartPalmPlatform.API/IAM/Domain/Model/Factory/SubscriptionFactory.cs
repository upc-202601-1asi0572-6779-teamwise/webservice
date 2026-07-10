using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Domain.Model.Enums;
using SmartPalmPlatform.API.IAM.Domain.Model.ValueObjects;

namespace SmartPalmPlatform.API.IAM.Domain.Model.Factory;

public static class SubscriptionFactory
{
    public static Subscription CreateSubscription(int userId, PlanType planType, decimal? customPrice = null)
    {
        var plan = SubscriptionPlanProvider.GetPlan(planType);

        var price = planType == PlanType.Custom && customPrice.HasValue
            ? customPrice.Value
            : plan.Price;

        var startDate = DateTime.UtcNow;
        var endDate = plan.Cycle switch
        {
            BillingCycle.Monthly => startDate.AddMonths(1),
            BillingCycle.Yearly => startDate.AddYears(1),
            _ => startDate.AddMonths(1),
        };

        var initialStatus = price > 0 ? SubscriptionStatus.Pending : SubscriptionStatus.Active;

        return new Subscription(
            userId,
            plan.Type,
            plan.Name,
            price,
            plan.Cycle,
            startDate,
            endDate,
            initialStatus);
    }
}
