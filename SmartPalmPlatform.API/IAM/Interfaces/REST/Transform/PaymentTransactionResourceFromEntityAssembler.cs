using SmartPalmPlatform.API.IAM.Domain.Model.Entities;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Transform;

public static class PaymentTransactionResourceFromEntityAssembler
{
    public static PaymentTransactionResource ToResourceFromEntity(PaymentTransaction entity)
    {
        return new PaymentTransactionResource(
            entity.PlanName,
            entity.PeriodStart.ToString("o"),
            entity.PeriodEnd.ToString("o"),
            entity.Amount,
            entity.Status.ToString(),
            entity.ProcessedAt.ToString("o")
        );
    }
}
