using SmartPalmPlatform.API.IAM.Domain.Model.Entities;
using SmartPalmPlatform.API.IAM.Domain.Model.Queries;

namespace SmartPalmPlatform.API.IAM.Domain.Services.QueryServices;

public interface IPaymentQueryService
{
    Task<IEnumerable<PaymentTransaction>> Handle(GetPaymentsByUserIdQuery query);
}
