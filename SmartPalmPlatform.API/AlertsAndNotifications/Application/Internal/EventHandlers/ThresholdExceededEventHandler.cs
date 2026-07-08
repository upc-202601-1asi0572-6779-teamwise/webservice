using SmartPalmPlatform.API.AlertsAndNotifications.Application.Internal.DomainServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Application.OutboundServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Application.Internal.EventHandlers;
using SmartPalmPlatform.API.Shared.Domain.Events;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Application.Internal.EventHandlers;

public class ThresholdExceededEventHandler(
    IAlertRepository alertRepository,
    IUserAlertSettingRepository userAlertSettingRepository,
    IUnitOfWork unitOfWork,
    AlertClassificationService classificationService,
    IFirebaseNotificationService firebaseNotificationService
) : IEventHandler<ThresholdExceededEvent>
{
    public async Task Handle(ThresholdExceededEvent notification, CancellationToken cancellationToken)
    {
        var level = classificationService.ClassifySeverity(
            notification.ReadingValue,
            (notification.ThresholdMin + notification.ThresholdMax) / 2
        );
        var message =
            $"{notification.SensorType} sensor on edge '{notification.EdgeDeviceMacAddress}' "
            + $"read {notification.ReadingValue}, outside threshold "
            + $"[{notification.ThresholdMin}, {notification.ThresholdMax}].";

        var alert = new Alert(notification.SensorType, 0, message, level);

        await alertRepository.AddAsync(alert);
        await unitOfWork.CompleteAsync();

        var setting = await userAlertSettingRepository.FindByUserIdAndSensorTypeAsync(0, notification.SensorType);
        if (setting is not null && setting.IsMuted)
            return;

        await firebaseNotificationService.SendNotificationAsync(alert);
    }
}
