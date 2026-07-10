using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Transform;

public static class AlertResourceFromEntityAssembler
{
    public static AlertResource ToResourceFromEntity(Alert entity)
    {
        return new AlertResource(
            entity.Id,
            entity.SensorType.ToString(),
            entity.Message,
            entity.Level.ToString(),
            entity.Status.ToString(),
            entity.Timestamp.ToString("o")
        );
    }
}
