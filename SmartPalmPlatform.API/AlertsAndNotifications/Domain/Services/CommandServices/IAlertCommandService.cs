using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Commands;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.CommandServices;

public interface IAlertCommandService
{
    Task<Alert> Handle(AcknowledgeAlertCommand command);
}
