using SmartPalmPlatform.API.IAM.Domain.Model.Commands;
using SmartPalmPlatform.API.IAM.Domain.Model.Enums;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Transform;

public static class CreateSubscriptionCommandFromResourceAssembler
{
    public static CreateSubscriptionCommand ToCommandFromResource(int userId, CreateSubscriptionResource resource)
    {
        var planType = Enum.Parse<PlanType>(resource.PlanType, ignoreCase: true);
        return new CreateSubscriptionCommand(userId, planType);
    }
}
