using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Transform;

public static class SubscriptionResourceFromEntityAssembler
{
    public static SubscriptionResource ToResourceFromEntity(Subscription entity)
    {
        return new SubscriptionResource(
            entity.PlanType.ToString(),
            entity.PlanName,
            entity.Price,
            entity.MaxHectares,
            entity.MaxSensors,
            entity.Status.ToString(),
            entity.StartDate.ToString("o"),
            entity.EndDate.ToString("o"),
            entity.BillingCycle.ToString(),
            entity.CreatedAt.ToString("o")
        );
    }
}
