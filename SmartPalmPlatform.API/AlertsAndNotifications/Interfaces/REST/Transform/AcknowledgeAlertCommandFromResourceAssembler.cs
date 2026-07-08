using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Commands;
using SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Interfaces.REST.Transform;

public static class AcknowledgeAlertCommandFromResourceAssembler
{
    public static AcknowledgeAlertCommand ToCommandFromResource(AcknowledgeAlertResource resource)
    {
        return new AcknowledgeAlertCommand(Guid.Parse(resource.alertId));
    }
}
