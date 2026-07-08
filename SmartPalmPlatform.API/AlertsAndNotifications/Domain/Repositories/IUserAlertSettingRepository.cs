using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Entities;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Domain.Repositories;

public interface IUserAlertSettingRepository
{
    Task AddAsync(UserAlertSetting setting);
    void Update(UserAlertSetting setting);
    Task<UserAlertSetting?> FindByUserIdAndSensorTypeAsync(int userId, SensorType sensorType);
    Task<IEnumerable<UserAlertSetting>> FindByUserIdAsync(int userId);
}
