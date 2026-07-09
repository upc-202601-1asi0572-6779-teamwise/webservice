using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Entities;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.QueryServices;

public interface IUserAlertSettingQueryService
{
    Task<IEnumerable<UserAlertSetting>> Handle(int userId);
    Task<UserAlertSetting?> Handle(int userId, SensorType sensorType);
}
