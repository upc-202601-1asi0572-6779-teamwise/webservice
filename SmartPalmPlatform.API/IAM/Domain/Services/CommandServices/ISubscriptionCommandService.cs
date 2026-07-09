using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Domain.Model.Commands;

namespace SmartPalmPlatform.API.IAM.Domain.Services.CommandServices;

public interface ISubscriptionCommandService
{
    Task<Subscription> Handle(CreateSubscriptionCommand command);
    Task<Subscription> Handle(CancelSubscriptionCommand command);
}
