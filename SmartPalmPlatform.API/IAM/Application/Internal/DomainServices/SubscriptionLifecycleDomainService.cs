using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Domain.Services.DomainServices;

namespace SmartPalmPlatform.API.IAM.Application.Internal.DomainServices;

public class SubscriptionLifecycleDomainService : ISubscriptionLifecycleDomainService
{
    public Task<bool> CanRenewSubscription(Subscription subscription)
    {
        var result = subscription.Status == Domain.Model.Enums.SubscriptionStatus.Active ||
                     subscription.Status == Domain.Model.Enums.SubscriptionStatus.Expired;
        return Task.FromResult(result);
    }

    public Task<bool> CanCancelSubscription(Subscription subscription)
    {
        var result = subscription.Status == Domain.Model.Enums.SubscriptionStatus.Active;
        return Task.FromResult(result);
    }

    public decimal GetRenewalPrice(Subscription subscription)
    {
        return subscription.Price;
    }
}
