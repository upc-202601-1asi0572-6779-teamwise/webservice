using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;

public interface IIotDeviceRepository : IBaseRepository<IotDevice>
{
    Task<IotDevice?> FindByMacAddress(string IotDeviceMacAddress);
}
