using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
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
            .Where(r => r.Status == RecommendationStatus.Pending)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recommendation>> FindBySectorIdAsync(int sectorId)
    {
        return await Context
            .Set<Recommendation>()
            .Where(r => r.SectorId == sectorId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recommendation>> FindBySectorIdAndStatusAsync(
        int sectorId,
        RecommendationStatus status
    )
    {
        return await Context
            .Set<Recommendation>()
            .Where(r => r.SectorId == sectorId && r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recommendation>> FindBySectorIdAndAgronomistIdAsync(
        int sectorId,
        int agronomistId
    )
    {
        return await Context
            .Set<Recommendation>()
            .Where(r => r.SectorId == sectorId && r.AgronomistId == agronomistId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recommendation>> FindBySectorIdAgronomistIdAndStatusAsync(
        int sectorId,
        int agronomistId,
        RecommendationStatus status
    )
    {
        return await Context
            .Set<Recommendation>()
            .Where(r =>
                r.SectorId == sectorId
                && r.AgronomistId == agronomistId
                && r.Status == status
            )
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recommendation>> FindByAgronomistIdAsync(int agronomistId)
    {
        return await Context
            .Set<Recommendation>()
            .Where(r => r.AgronomistId == agronomistId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recommendation>> FindGeneralAsync()
    {
        return await Context
            .Set<Recommendation>()
            .Where(r => r.SectorId == null && r.Type == RecommendationType.General)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recommendation>> FindGeneralAndStatusAsync(RecommendationStatus status)
    {
        return await Context
            .Set<Recommendation>()
            .Where(r => r.SectorId == null && r.Type == RecommendationType.General && r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recommendation>> FindByReportIdAsync(int reportId)
    {
        return await Context
            .Set<Recommendation>()
            .Where(r => r.ReportId == reportId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}

