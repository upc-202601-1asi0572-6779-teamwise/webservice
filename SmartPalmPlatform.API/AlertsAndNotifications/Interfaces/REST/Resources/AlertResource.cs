namespace SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Resources;

public record AlertResource(
    int id,
    string sensorType,
    string message,
    string level,
    string status,
    string timestamp
);
