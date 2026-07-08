using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Commands;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Repositories;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.CommandServices;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Application.Internal.CommandServices;

public class AlertCommandService(
    IAlertRepository alertRepository,
    IUnitOfWork unitOfWork) : IAlertCommandService
{
    public async Task<Alert> Handle(AcknowledgeAlertCommand command)
    {
        var alert = await alertRepository.FindByGuidAsync(command.AlertId);
        if (alert is null)
            throw new KeyNotFoundException($"Alert {command.AlertId} not found.");

        alert.Acknowledge();
        await unitOfWork.CompleteAsync();
        return alert;
    }
}
