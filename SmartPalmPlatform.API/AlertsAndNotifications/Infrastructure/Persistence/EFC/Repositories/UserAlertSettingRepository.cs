using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Entities;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Infrastructure.Persistence.EFC.Repositories;

public class UserAlertSettingRepository(AppDbContext context)
    : BaseRepository<UserAlertSetting>(context), IUserAlertSettingRepository
{
    public async Task<UserAlertSetting?> FindByUserIdAndSensorTypeAsync(int userId, SensorType sensorType)
    {
        return await Context.Set<UserAlertSetting>()
            .FirstOrDefaultAsync(s => s.UserId == userId && s.SensorType == sensorType);
    }

    public async Task<IEnumerable<UserAlertSetting>> FindByUserIdAsync(int userId)
    {
        return await Context.Set<UserAlertSetting>()
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }
}
