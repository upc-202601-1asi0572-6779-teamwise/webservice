using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;

public interface IEdgeRegistryRepository : IBaseRepository<EdgeRegistry>
{
    Task<List<EdgeRegistry>> FindByEdgeMacAddress(string EdgeMacAddress);
    Task<List<EdgeRegistry>> FindByIotDeviceMacAddress(string IotDeviceMacAddress);
    Task<EdgeRegistry?> FindByEdgeAndIotMacAddresses(
        string EdgeMacAddress,
        string IotDeviceMacAddress
    );
}
