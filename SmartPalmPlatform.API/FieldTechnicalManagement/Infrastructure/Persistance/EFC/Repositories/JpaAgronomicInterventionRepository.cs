using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.FieldTechnicalManagement.Infrastructure.Persistance.EFC.Repositories;

public class JpaAgronomicInterventionRepository(AppDbContext context)
    : BaseRepository<AgronomicIntervention>(context),
        IAgronomicInterventionRepository
{
    public async Task<IEnumerable<AgronomicIntervention>> FindBySectorIdAsync(
        int sectorId,
        DateTime? startDate = null,
        DateTime? endDate = null
    )
    {
        var query = Context
            .Set<AgronomicIntervention>()
            .Where(a => a.SectorId == sectorId)
            .OrderByDescending(a => a.ExecutionDate)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(a => a.ExecutionDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(a => a.ExecutionDate <= endDate.Value);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<AgronomicIntervention>> FindByRecommendationIdAsync(
        int recommendationId
    )
    {
        return await Context
            .Set<AgronomicIntervention>()
            .Where(a => a.RecommendationId == recommendationId)
            .OrderByDescending(a => a.ExecutionDate)
            .ToListAsync();
    }
}
