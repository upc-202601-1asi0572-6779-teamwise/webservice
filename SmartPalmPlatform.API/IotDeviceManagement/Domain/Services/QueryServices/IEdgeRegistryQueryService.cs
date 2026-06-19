using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Queries;

namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.QueryServices;

public interface IEdgeRegistryQueryService
{
    Task<EdgeDevice> Handle(EdgeRegistryQuery query);
}
