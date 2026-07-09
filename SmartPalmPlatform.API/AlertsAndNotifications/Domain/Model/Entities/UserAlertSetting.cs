using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Entities;

public class UserAlertSetting
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public SensorType SensorType { get; private set; }
    public bool IsMuted { get; private set; }

    public UserAlertSetting(int userId, SensorType sensorType, bool isMuted)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId must be greater than zero.");

        UserId = userId;
        SensorType = sensorType;
        IsMuted = isMuted;
    }

    private UserAlertSetting() { }

    public void UpdateMute(bool isMuted)
    {
        IsMuted = isMuted;
    }
}
