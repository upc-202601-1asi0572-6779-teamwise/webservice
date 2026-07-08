using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Queries;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Repositories;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.QueryServices;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Application.Internal.QueryServices;

public class AlertQueryService(IAlertRepository alertRepository) : IAlertQueryService
{
    public async Task<IEnumerable<Alert>> Handle(GetAlertsByUserIdQuery query)
    {
        return await alertRepository.FindByUserIdAsync(query.UserId);
    }

    public async Task<Alert?> Handle(GetAlertByIdQuery query)
    {
        return await alertRepository.FindByGuidAsync(query.AlertId);
    }
}
