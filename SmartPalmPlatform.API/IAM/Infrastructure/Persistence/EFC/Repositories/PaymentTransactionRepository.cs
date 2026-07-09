using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.IAM.Domain.Model.Entities;
using SmartPalmPlatform.API.IAM.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.IAM.Infrastructure.Persistence.EFC.Repositories;

public class PaymentTransactionRepository(AppDbContext context)
    : BaseRepository<PaymentTransaction>(context), IPaymentTransactionRepository
{
    public async Task<IEnumerable<PaymentTransaction>> FindByUserIdAsync(int userId)
    {
        return await Context.Set<PaymentTransaction>()
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }
}
