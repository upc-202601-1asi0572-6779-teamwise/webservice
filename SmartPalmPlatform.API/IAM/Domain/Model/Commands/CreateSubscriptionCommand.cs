using SmartPalmPlatform.API.IAM.Domain.Model.Enums;

namespace SmartPalmPlatform.API.IAM.Domain.Model.Commands;

public record CreateSubscriptionCommand(int UserId, PlanType PlanType);
