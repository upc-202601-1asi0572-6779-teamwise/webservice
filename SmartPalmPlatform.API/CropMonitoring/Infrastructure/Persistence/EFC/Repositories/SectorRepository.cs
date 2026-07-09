using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Entities;
using SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.CropMonitoring.Infrastructure.Persistence.EFC.Repositories;

public class SectorRepository(AppDbContext context)
    : BaseRepository<Sector>(context), ISectorRepository
{
    public async Task<List<Sector>> FindByPlantationIdAsync(int plantationId)
    {
        return await Context.Set<Sector>()
            .Where(s => s.PlantationId == plantationId)
            .ToListAsync();
    }

    public async Task<Sector?> FindByIotDeviceMacAddressAsync(string iotDeviceMacAddress)
    {
        return await Context.Set<Sector>()
            .FirstOrDefaultAsync(s => s.IotDeviceMacAddress == iotDeviceMacAddress);
    }
}
