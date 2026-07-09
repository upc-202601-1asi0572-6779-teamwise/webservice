using SmartPalmPlatform.API.IAM.Domain.Model.Entities;
using SmartPalmPlatform.API.IAM.Domain.Services.DomainServices;

namespace SmartPalmPlatform.API.IAM.Application.Internal.DomainServices;

public class LocalPaymentStrategy : IPaymentStrategy
{
    public Task<PaymentResult> ProcessPaymentAsync(PaymentTransaction transaction)
    {
        var result = new PaymentResult(true, Guid.NewGuid().ToString(), null);
        return Task.FromResult(result);
    }
}
