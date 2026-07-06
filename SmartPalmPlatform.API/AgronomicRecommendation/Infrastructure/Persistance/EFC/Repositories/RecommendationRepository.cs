using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Repositories;

public class RecommendationRepository(AppDbContext context)
    : BaseRepository<Recommendation>(context),
        IRecommendationRepository
{
    public async Task<IEnumerable<Recommendation>> FindPendingAsync()
    {
        return await Context
            .Set<Recommendation>()
            .Where(recommendation => recommendation.Status == RecommendationStatus.Pending)
            .OrderByDescending(recommendation => recommendation.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recommendation>> FindByPlantationIdAsync(int plantationId)
    {
        return await Context
            .Set<Recommendation>()
            .Where(recommendation => recommendation.PlantationId == plantationId)
            .OrderByDescending(recommendation => recommendation.CreatedAt)
            .ToListAsync();
    }

    public async Task AddInterventionAsync(AgronomicIntervention intervention)
    {
        await Context.Set<AgronomicIntervention>().AddAsync(intervention);
    }

    public async Task<IEnumerable<AgronomicIntervention>> FindInterventionsByRecommendationIdAsync(
        int recommendationId
    )
    {
        return await Context
            .Set<AgronomicIntervention>()
            .Where(intervention => intervention.RecommendationId == recommendationId)
            .OrderByDescending(intervention => intervention.ExecutionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recommendation>> FindByAgronomistIdAsync(int agronomistId)
    {
        return await Context
            .Set<Recommendation>()
            .Where(recommendation => recommendation.AgronomistId == agronomistId)
            .OrderByDescending(recommendation => recommendation.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recommendation>> FindByPlantationIdAndStatusAsync(
        int plantationId,
        RecommendationStatus status
    )
    {
        return await Context
            .Set<Recommendation>()
            .Where(r => r.PlantationId == plantationId && r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recommendation>> FindByPlantationIdAndAgronomistIdAsync(
        int plantationId,
        int agronomistId
    )
    {
        return await Context
            .Set<Recommendation>()
            .Where(r => r.PlantationId == plantationId && r.AgronomistId == agronomistId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recommendation>> FindByPlantationIdAgronomistIdAndStatusAsync(
        int plantationId,
        int agronomistId,
        RecommendationStatus status
    )
    {
        return await Context
            .Set<Recommendation>()
            .Where(r =>
                r.PlantationId == plantationId
                && r.AgronomistId == agronomistId
                && r.Status == status
            )
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}

