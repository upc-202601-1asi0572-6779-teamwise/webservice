using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Model.Queries;

namespace SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.QueryServices;

public interface IAlertQueryService
{
    Task<IEnumerable<Alert>> Handle(GetAlertsByUserIdQuery query);
    Task<Alert?> Handle(GetAlertByIdQuery query);
}
