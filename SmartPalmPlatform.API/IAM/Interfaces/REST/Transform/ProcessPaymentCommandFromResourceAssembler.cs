using SmartPalmPlatform.API.IAM.Domain.Model.Commands;
using SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Transform;

public static class ProcessPaymentCommandFromResourceAssembler
{
    public static ProcessPaymentCommand ToCommandFromResource(int userId, ProcessPaymentResource resource)
    {
        return new ProcessPaymentCommand(userId, resource.amount);
    }
}
