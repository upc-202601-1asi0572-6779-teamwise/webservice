using SmartPalmPlatform.API.AlertsAndNotifications.Application.OutboundServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Application.Internal.EventHandlers;

public class AlertGeneratedEventHandler(IFirebaseNotificationService firebaseNotificationService)
{
    public async Task Handle(Alert alert)
    {
        await firebaseNotificationService.SendNotificationAsync(alert);
    }
}
