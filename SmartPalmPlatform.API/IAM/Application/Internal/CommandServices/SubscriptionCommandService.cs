using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Domain.Model.Commands;
using SmartPalmPlatform.API.IAM.Domain.Model.Enums;
using SmartPalmPlatform.API.IAM.Domain.Model.Factory;
using SmartPalmPlatform.API.IAM.Domain.Repositories;
using SmartPalmPlatform.API.IAM.Domain.Services.CommandServices;
using SmartPalmPlatform.API.IAM.Domain.Services.DomainServices;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.IAM.Application.Internal.CommandServices;

public class SubscriptionCommandService(
    ISubscriptionRepository subscriptionRepository,
    IUserRepository userRepository,
    ISubscriptionLifecycleDomainService lifecycleDomainService,
    IUnitOfWork unitOfWork) : ISubscriptionCommandService
{
    public async Task<Subscription> Handle(CreateSubscriptionCommand command)
    {
        var user = await userRepository.FindByIdAsync(command.UserId);
        if (user is null)
            throw new KeyNotFoundException($"User {command.UserId} not found.");

        var existing = await subscriptionRepository.FindByUserIdAsync(command.UserId);
        if (existing is not null && existing.Status is SubscriptionStatus.Active or SubscriptionStatus.Pending)
            throw new InvalidOperationException("User already has an active or pending subscription.");

        var subscription = SubscriptionFactory.CreateSubscription(command.UserId, command.PlanType);
        await subscriptionRepository.AddAsync(subscription);
        await unitOfWork.CompleteAsync();
        return subscription;
    }

    public async Task<Subscription> Handle(CancelSubscriptionCommand command)
    {
        var subscription = await subscriptionRepository.FindByUserIdAsync(command.UserId);
        if (subscription is null)
            throw new KeyNotFoundException($"No subscription found for user {command.UserId}.");

        if (!await lifecycleDomainService.CanCancelSubscription(subscription))
            throw new InvalidOperationException("Subscription cannot be cancelled.");

        subscription.Cancel();
        subscriptionRepository.Update(subscription);
        await unitOfWork.CompleteAsync();
        return subscription;
    }
}
