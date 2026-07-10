using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Repositories;

public class ReportRepository(AppDbContext context)
    : BaseRepository<Report>(context),
        IReportRepository
{
    public async Task<IEnumerable<Report>> FindBySectorIdAsync(int sectorId)
    {
        return await Context
            .Set<Report>()
            .Where(r => r.SectorId == sectorId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Report>> FindByAgronomistIdAsync(int agronomistId)
    {
        return await Context
            .Set<Report>()
            .Where(r => r.AgronomistId == agronomistId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}