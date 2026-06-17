using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Queries;

namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Services;

public interface IDeviceStatusQueryService
{
    Task<IotDevice> Handle(ConnectiviyStatusQuery query);
    Task<IotDevice> Handle(ActivationStatusQuery query);
}
