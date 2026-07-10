using SmartPalmPlatform.API.IAM.Domain.Model.ValueObjects;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Transform;

public static class PlanResourceFromSubscriptionPlanAssembler
{
    public static PlanResource ToResourceFromSubscriptionPlan(SubscriptionPlan plan)
    {
        return new PlanResource(
            plan.Type.ToString(),
            plan.Name,
            plan.Price,
            plan.Cycle.ToString(),
            plan.MaxHectares,
            plan.MaxSensors,
            plan.MaxPlantationHistory
        );
    }
}
