using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Entities;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.CommandServices;

public interface IUserAlertSettingCommandService
{
    Task<UserAlertSetting> Handle(int userId, SensorType sensorType, bool isMuted);
}
