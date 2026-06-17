using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Queries;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services;

namespace SmartPalmPlatform.API.IotDeviceManagement.Application.QueryServices;

public class DeviceStatusQueryService(IIotDeviceRepository deviceRepository)
    : IDeviceStatusQueryService
{
    public async Task<IotDevice> Handle(ConnectiviyStatusQuery query)
    {
        var device = await deviceRepository.FindBySerialNumberAsync(query.serial);

        if (device is null)
        {
            throw new Exception("Device not found");
        }

        return device;
    }

    public async Task<IotDevice> Handle(ActivationStatusQuery query)
    {
        var device = await deviceRepository.FindBySerialNumberAsync(query.serial);

        if (device is null)
        {
            throw new Exception("Device not found");
        }

        return device;
    }
}
