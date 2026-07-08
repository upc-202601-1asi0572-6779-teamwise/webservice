namespace SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Resources;

public record AlertResource(
    string sensorType,
    string message,
    string level,
    string status,
    string timestamp
);
