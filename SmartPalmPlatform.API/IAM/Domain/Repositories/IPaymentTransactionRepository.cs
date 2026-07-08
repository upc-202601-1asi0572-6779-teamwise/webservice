using SmartPalmPlatform.API.IAM.Domain.Model.Entities;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.IAM.Domain.Repositories;

public interface IPaymentTransactionRepository : IBaseRepository<PaymentTransaction>
{
    Task<IEnumerable<PaymentTransaction>> FindByUserIdAsync(int userId);
}
