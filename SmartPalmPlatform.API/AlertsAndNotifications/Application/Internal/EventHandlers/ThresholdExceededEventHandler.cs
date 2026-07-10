using SmartPalmPlatform.API.AlertsAndNotifications.Application.Internal.DomainServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Application.OutboundServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Repositories;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.ACL;
using SmartPalmPlatform.API.Shared.Application.Internal.EventHandlers;
using SmartPalmPlatform.API.Shared.Domain.Events;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Application.Internal.EventHandlers;

public class ThresholdExceededEventHandler(
    IAlertRepository alertRepository,
    IUserAlertSettingRepository userAlertSettingRepository,
    IUnitOfWork unitOfWork,
    AlertClassificationService classificationService,
    IFirebaseNotificationService firebaseNotificationService,
    ICropMonitoringFacade cropMonitoringFacade
) : IEventHandler<ThresholdExceededEvent>
{
    public async Task Handle(ThresholdExceededEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[INFO] [Alerts] [EventHandler] Threshold exceeded: edge={notification.EdgeDeviceMacAddress}, iot={notification.IotDeviceMacAddress}, sensor={notification.SensorType}, value={notification.ReadingValue}, range=[{notification.ThresholdMin},{notification.ThresholdMax}]");

        var level = classificationService.ClassifySeverity(
            notification.ReadingValue,
            (notification.ThresholdMin + notification.ThresholdMax) / 2
        );
        var message =
            $"{notification.SensorType} sensor on edge '{notification.EdgeDeviceMacAddress}' "
            + $"read {notification.ReadingValue}, outside threshold "
            + $"[{notification.ThresholdMin}, {notification.ThresholdMax}].";

        var palmGrowerId = await cropMonitoringFacade.GetPalmGrowerIdByIotDeviceMacAsync(
            notification.IotDeviceMacAddress
        );
        var resolvedUserId = palmGrowerId ?? 0;
        if (palmGrowerId is null)
        {
            Console.WriteLine($"[WARN] [Alerts] [EventHandler] Could not resolve PalmGrower for IoT device '{notification.IotDeviceMacAddress}'. Alert will be persisted with userId=0 as fallback.");
        }

        var alert = new Alert(notification.SensorType, resolvedUserId, message, level);
        Console.WriteLine($"[INFO] [Alerts] [EventHandler] Creating alert level={level} for userId={resolvedUserId}: {message}");

        await alertRepository.AddAsync(alert);
        await unitOfWork.CompleteAsync();
        Console.WriteLine($"[INFO] [Alerts] [EventHandler] Alert #{alert.Id} saved.");

        var setting = await userAlertSettingRepository.FindByUserIdAndSensorTypeAsync(resolvedUserId, notification.SensorType);
        if (setting is not null && setting.IsMuted)
        {
            Console.WriteLine($"[INFO] [Alerts] [EventHandler] Notifications muted for sensor type {notification.SensorType} (userId={resolvedUserId}), skipping Firebase.");
            return;
        }

        Console.WriteLine($"[INFO] [Alerts] [EventHandler] Sending Firebase notification for alert #{alert.Id}...");
        await firebaseNotificationService.SendNotificationAsync(alert);
        Console.WriteLine($"[INFO] [Alerts] [EventHandler] Firebase notification sent for alert #{alert.Id}.");
    }
}
