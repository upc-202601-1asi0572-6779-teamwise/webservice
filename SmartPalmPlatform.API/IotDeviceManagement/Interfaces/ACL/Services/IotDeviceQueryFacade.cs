using SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.ACL.Services;

public class IotDeviceQueryFacade(
    IIotDeviceRepository iotDeviceRepository
) : IIotDeviceQueryFacade
{
    public async Task<bool> ExistsByMacAddress(string macAddress)
    {
        var device = await iotDeviceRepository.FindByMacAddress(macAddress);
        return device is not null;
    }

    public async Task<int?> GetOwnerUserIdByMacAddress(string macAddress)
    {
        var device = await iotDeviceRepository.FindByMacAddress(macAddress);
        return device?.UserId;
    }
}
