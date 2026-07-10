using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Infrastructure.Persistence.EFC.Repositories;

public class AlertRepository(AppDbContext context)
    : BaseRepository<Alert>(context), IAlertRepository
{
    public async Task<Alert?> FindByIdAsync(int alertId)
    {
        return await Context.Set<Alert>().FirstOrDefaultAsync(a => a.Id == alertId);
    }

    public async Task<IEnumerable<Alert>> FindByUserIdAsync(int userId)
    {
        return await Context.Set<Alert>()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> FindBySensorTypeAsync(SensorType sensorType)
    {
        return await Context.Set<Alert>()
            .Where(a => a.SensorType == sensorType)
            .ToListAsync();
    }

    public async Task<IEnumerable<Alert>> FindAllAsync()
    {
        return await Context.Set<Alert>()
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }
}
