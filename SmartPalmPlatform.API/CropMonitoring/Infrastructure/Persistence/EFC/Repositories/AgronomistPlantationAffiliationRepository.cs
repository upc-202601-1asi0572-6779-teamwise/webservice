using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Entities;
using SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.CropMonitoring.Infrastructure.Persistence.EFC.Repositories;

public class AgronomistPlantationAffiliationRepository(AppDbContext context)
    : BaseRepository<AgronomistPlantationAffiliation>(context),
        IAgronomistPlantationAffiliationRepository
{
    public async Task<List<AgronomistPlantationAffiliation>> FindByAgronomistIdAsync(int agronomistId)
    {
        return await Context.Set<AgronomistPlantationAffiliation>()
            .Where(a => a.AgronomistId == agronomistId)
            .OrderByDescending(a => a.AffiliatedAt)
            .ToListAsync();
    }

    public async Task<List<AgronomistPlantationAffiliation>> FindByPlantationIdAsync(int plantationId)
    {
        return await Context.Set<AgronomistPlantationAffiliation>()
            .Where(a => a.PlantationId == plantationId)
            .OrderByDescending(a => a.AffiliatedAt)
            .ToListAsync();
    }

    public async Task<AgronomistPlantationAffiliation?> FindByAgronomistAndPlantationAsync(int agronomistId, int plantationId)
    {
        return await Context.Set<AgronomistPlantationAffiliation>()
            .FirstOrDefaultAsync(a =>
                a.AgronomistId == agronomistId
                && a.PlantationId == plantationId
                && !a.DetachedAt.HasValue
            );
    }
}