using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Application.OutboundServices;

public interface IFirebaseNotificationService
{
    Task SendNotificationAsync(Alert alert);
}
