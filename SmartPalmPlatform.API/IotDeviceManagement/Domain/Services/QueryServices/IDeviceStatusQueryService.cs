using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Queries;

namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.QueryServices;

public interface IDeviceStatusQueryService
{
    Task<EdgeDevice> Handle(ConnectiviyStatusQuery query);
    Task<Tuple<EdgeDevice, List<EdgeRegistry>>> Handle(EdgeRegistryQuery query);
    Task<IEnumerable<EdgeDevice>> Handle(GetAllEdgeGatewaysQuery query);
}
