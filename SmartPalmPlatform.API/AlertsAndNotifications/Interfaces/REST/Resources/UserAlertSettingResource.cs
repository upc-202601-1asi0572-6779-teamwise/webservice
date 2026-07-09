namespace SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Resources;

public record UserAlertSettingResource(
    string sensorType,
    bool isMuted
);
