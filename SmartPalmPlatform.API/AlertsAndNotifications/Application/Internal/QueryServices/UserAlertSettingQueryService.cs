using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Entities;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Repositories;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.QueryServices;
using SmartPalmPlatform.API.IAM.Interfaces.ACL;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Application.Internal.QueryServices;

public class UserAlertSettingQueryService(
    IUserAlertSettingRepository repository,
    IIamContextFacade iamContextFacade
) : IUserAlertSettingQueryService
{
    public async Task<IEnumerable<UserAlertSetting>> Handle(int userId)
    {
        var username = await iamContextFacade.FetchUsernameByUserId(userId);
        if (string.IsNullOrEmpty(username))
            throw new KeyNotFoundException($"User {userId} not found.");

        return await repository.FindByUserIdAsync(userId);
    }

    public async Task<UserAlertSetting?> Handle(int userId, SensorType sensorType)
    {
        var username = await iamContextFacade.FetchUsernameByUserId(userId);
        if (string.IsNullOrEmpty(username))
            throw new KeyNotFoundException($"User {userId} not found.");

        return await repository.FindByUserIdAndSensorTypeAsync(userId, sensorType);
    }
}
