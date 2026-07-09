using SmartPalmPlatform.API.IAM.Domain.Model.Entities;

namespace SmartPalmPlatform.API.IAM.Domain.Services.DomainServices;

public record PaymentResult(bool Success, string TransactionId, string? ErrorMessage);

public interface IPaymentStrategy
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentTransaction transaction);
}
