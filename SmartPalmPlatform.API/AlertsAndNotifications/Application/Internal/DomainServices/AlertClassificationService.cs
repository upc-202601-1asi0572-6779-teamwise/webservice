using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Enums;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Application.Internal.DomainServices;

public class AlertClassificationService
{
    public AlertLevel ClassifySeverity(double value, double threshold)
    {
        var deviation = Math.Abs(value - threshold) / threshold;

        return deviation switch
        {
            < 0.1 => AlertLevel.Informational,
            < 0.3 => AlertLevel.Warning,
            _ => AlertLevel.Critical,
        };
    }
}
