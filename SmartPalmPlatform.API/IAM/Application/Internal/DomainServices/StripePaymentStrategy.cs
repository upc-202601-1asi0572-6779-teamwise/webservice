using SmartPalmPlatform.API.IAM.Domain.Model.Entities;
using SmartPalmPlatform.API.IAM.Domain.Services.DomainServices;

namespace SmartPalmPlatform.API.IAM.Application.Internal.DomainServices;

public class StripePaymentStrategy : IPaymentStrategy
{
    public Task<PaymentResult> ProcessPaymentAsync(PaymentTransaction transaction)
    {
        throw new NotImplementedException("Stripe integration pending.");
    }
}
