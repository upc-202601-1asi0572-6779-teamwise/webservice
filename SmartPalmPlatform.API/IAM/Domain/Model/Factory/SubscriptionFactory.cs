using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Domain.Model.Enums;
using SmartPalmPlatform.API.IAM.Domain.Model.ValueObjects;

namespace SmartPalmPlatform.API.IAM.Domain.Model.Factory;

public static class SubscriptionFactory
{
    public static Subscription CreateSubscription(int userId, PlanType planType)
    {
        var plan = SubscriptionPlanProvider.GetPlan(planType);

        var startDate = DateTime.UtcNow;
        var endDate = plan.Cycle switch
        {
            BillingCycle.Monthly => startDate.AddMonths(1),
            BillingCycle.Yearly => startDate.AddYears(1),
            _ => startDate.AddMonths(1),
        };

        var initialStatus = plan.Price > 0 ? SubscriptionStatus.Pending : SubscriptionStatus.Active;

        return new Subscription(
            userId,
            plan.Type,
            plan.Name,
            plan.Price,
            plan.MaxHectares,
            plan.MaxSensors,
            plan.Cycle,
            startDate,
            endDate,
            initialStatus);
    }
}
