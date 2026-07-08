using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Entities;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Repositories;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.CommandServices;
using SmartPalmPlatform.API.IAM.Interfaces.ACL;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Application.Internal.CommandServices;

public class UserAlertSettingCommandService(
    IUserAlertSettingRepository repository,
    IUnitOfWork unitOfWork,
    IIamContextFacade iamContextFacade
) : IUserAlertSettingCommandService
{
    public async Task<UserAlertSetting> Handle(int userId, SensorType sensorType, bool isMuted)
    {
        var username = await iamContextFacade.FetchUsernameByUserId(userId);
        if (string.IsNullOrEmpty(username))
            throw new KeyNotFoundException($"User {userId} not found.");

        var existing = await repository.FindByUserIdAndSensorTypeAsync(userId, sensorType);
        if (existing is not null)
        {
            existing.UpdateMute(isMuted);
            repository.Update(existing);
        }
        else
        {
            existing = new UserAlertSetting(userId, sensorType, isMuted);
            await repository.AddAsync(existing);
        }

        await unitOfWork.CompleteAsync();
        return existing;
    }
}
