using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Aggregates;
using SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.CropMonitoring.Infrastructure.Persistence.EFC.Repositories;

public class PlantationRepository(AppDbContext context)
    : BaseRepository<Plantation>(context), IPlantationRepository
{
    public async Task<List<Plantation>> FindByPalmGrowerIdAsync(int palmGrowerId)
    {
        return await Context.Set<Plantation>()
            .Where(p => p.PalmGrowerId == palmGrowerId)
            .ToListAsync();
    }

    public async Task<Plantation?> FindByIdWithSectorsAsync(int id)
    {
        return await Context.Set<Plantation>()
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
