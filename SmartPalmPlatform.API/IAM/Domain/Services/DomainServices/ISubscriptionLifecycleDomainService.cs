using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;

namespace SmartPalmPlatform.API.IAM.Domain.Services.DomainServices;

public interface ISubscriptionLifecycleDomainService
{
    Task<bool> CanRenewSubscription(Subscription subscription);
    Task<bool> CanCancelSubscription(Subscription subscription);
    decimal GetRenewalPrice(Subscription subscription);
}
