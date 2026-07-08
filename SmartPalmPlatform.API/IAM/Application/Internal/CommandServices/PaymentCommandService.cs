using SmartPalmPlatform.API.IAM.Domain.Model.Entities;
using SmartPalmPlatform.API.IAM.Domain.Model.Commands;
using SmartPalmPlatform.API.IAM.Domain.Repositories;
using SmartPalmPlatform.API.IAM.Domain.Services.CommandServices;
using SmartPalmPlatform.API.IAM.Domain.Services.DomainServices;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.IAM.Application.Internal.CommandServices;

public class PaymentCommandService(
    IPaymentTransactionRepository paymentTransactionRepository,
    ISubscriptionRepository subscriptionRepository,
    IPaymentStrategy paymentStrategy,
    IUnitOfWork unitOfWork) : IPaymentCommandService
{
    public async Task<PaymentTransaction> Handle(ProcessPaymentCommand command)
    {
        var subscription = await subscriptionRepository.FindByUserIdAsync(command.UserId);
        if (subscription is null)
            throw new KeyNotFoundException($"No subscription found for user {command.UserId}.");

        if (subscription.Status == Domain.Model.Enums.SubscriptionStatus.Active)
            throw new InvalidOperationException("Subscription is already active. No payment needed.");

        if (subscription.Status != Domain.Model.Enums.SubscriptionStatus.Pending)
            throw new InvalidOperationException("Only pending subscriptions can be paid.");

        if (command.Amount != subscription.Price)
            throw new ArgumentException($"Payment amount must match the plan price ({subscription.Price:C}).");

        var transaction = new PaymentTransaction(
            command.UserId,
            subscription.PlanName,
            subscription.StartDate,
            subscription.EndDate,
            command.Amount);
        await paymentTransactionRepository.AddAsync(transaction);

        var result = await paymentStrategy.ProcessPaymentAsync(transaction);

        if (result.Success)
        {
            transaction.Complete(result.TransactionId);
            if (subscription.Status == Domain.Model.Enums.SubscriptionStatus.Pending)
            {
                subscription.Activate();
            }
        }
        else
        {
            transaction.Fail();
        }

        await unitOfWork.CompleteAsync();
        return transaction;
    }
}
