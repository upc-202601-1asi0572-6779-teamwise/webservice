using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Domain.Model.Queries;

namespace SmartPalmPlatform.API.IAM.Domain.Services.QueryServices;

public interface ISubscriptionQueryService
{
    Task<Subscription?> Handle(GetSubscriptionByUserIdQuery query);
    Task<Subscription?> Handle(GetSubscriptionByIdQuery query);
    Task<IEnumerable<Subscription>> Handle(GetAllSubscriptionsQuery query);
}
