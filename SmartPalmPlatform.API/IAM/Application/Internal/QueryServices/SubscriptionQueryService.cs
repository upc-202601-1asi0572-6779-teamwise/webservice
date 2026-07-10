using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Domain.Model.Queries;
using SmartPalmPlatform.API.IAM.Domain.Repositories;
using SmartPalmPlatform.API.IAM.Domain.Services.QueryServices;

namespace SmartPalmPlatform.API.IAM.Application.Internal.QueryServices;

public class SubscriptionQueryService(ISubscriptionRepository subscriptionRepository) : ISubscriptionQueryService
{
    public async Task<Subscription?> Handle(GetSubscriptionByUserIdQuery query)
    {
        return await subscriptionRepository.FindByUserIdAsync(query.UserId);
    }

    public async Task<Subscription?> Handle(GetSubscriptionByIdQuery query)
    {
        return await subscriptionRepository.FindByIdAsync(query.SubscriptionId);
    }

    public async Task<IEnumerable<Subscription>> Handle(GetAllSubscriptionsQuery query)
    {
        return await subscriptionRepository.ListAsync();
    }
}
