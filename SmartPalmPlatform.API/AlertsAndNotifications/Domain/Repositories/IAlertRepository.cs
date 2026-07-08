using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Domain.Repositories;

public interface IAlertRepository
{
    Task AddAsync(Alert alert);
    Task<Alert?> FindByGuidAsync(Guid alertId);
    Task<IEnumerable<Alert>> FindByUserIdAsync(int userId);
    Task<IEnumerable<Alert>> FindBySensorTypeAsync(SensorType sensorType);
}
