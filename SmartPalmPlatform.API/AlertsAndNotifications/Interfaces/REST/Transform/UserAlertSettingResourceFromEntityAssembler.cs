using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Entities;
using SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Transform;

public static class UserAlertSettingResourceFromEntityAssembler
{
    public static UserAlertSettingResource ToResourceFromEntity(UserAlertSetting entity)
    {
        return new UserAlertSettingResource(
            entity.SensorType.ToString(),
            entity.IsMuted
        );
    }
}
