using SmartPalmPlatform.API.IAM.Domain.Model.Entities;
using SmartPalmPlatform.API.IAM.Domain.Model.Queries;
using SmartPalmPlatform.API.IAM.Domain.Repositories;
using SmartPalmPlatform.API.IAM.Domain.Services.QueryServices;

namespace SmartPalmPlatform.API.IAM.Application.Internal.QueryServices;

public class PaymentQueryService(IPaymentTransactionRepository paymentTransactionRepository) : IPaymentQueryService
{
    public async Task<IEnumerable<PaymentTransaction>> Handle(GetPaymentsByUserIdQuery query)
    {
        return await paymentTransactionRepository.FindByUserIdAsync(query.UserId);
    }
}
