using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Enums;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;

public class Alert
{
    public Guid Id { get; private set; }
    public SensorType SensorType { get; private set; }
    public int UserId { get; private set; }
    public string Message { get; private set; }
    public AlertLevel Level { get; private set; }
    public AlertStatus Status { get; private set; }
    public DateTime Timestamp { get; private set; }

    public Alert(SensorType sensorType, int userId, string message, AlertLevel level)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty.");

        SensorType = sensorType;
        UserId = userId;
        Message = message;
        Level = level;
        Status = AlertStatus.Active;
        Timestamp = DateTime.UtcNow;
    }

    private Alert()
    {
        Message = string.Empty;
    }

    public void Acknowledge()
    {
        if (Status != AlertStatus.Active)
            throw new InvalidOperationException("Only active alerts can be acknowledged.");
        Status = AlertStatus.Acknowledged;
    }
}
