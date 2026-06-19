using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;

public interface IEdgeDeviceRepository : IBaseRepository<EdgeDevice>
{
    Task<EdgeDevice?> FindByMacAddress(string EdgeDeviceMacAddress);
}
