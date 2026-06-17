using SmartPalmPlatform.API.Shared.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    /// <inheritdoc />
    public async Task CompleteAsync()
    {
        await context.SaveChangesAsync();
    }
}
