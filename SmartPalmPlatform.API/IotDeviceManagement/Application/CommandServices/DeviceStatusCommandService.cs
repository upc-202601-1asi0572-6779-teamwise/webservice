using SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.IotDeviceManagement.Application.CommandServices;

public class DeviceStatusCommandService(IUnitOfWork uow, IIotDeviceRepository deviceRepository)
    : IDeviceStatusCommandService
{
    public async Task Handle(RegisterDeviceCommand command)
    {
        var device = await deviceRepository.FindBySerialNumberAsync(command.serial);
        if (device is not null)
        {
            throw new Exception("Device already registered");
        }

        // TODO: Validate monitoringZoneId command
        //

        device = new IotDevice(command.serial, command.monitoringZoneId);

        device.Activate();

        await deviceRepository.AddAsync(device);
        await uow.CompleteAsync();
    }

    public async Task Handle(ActivateDeviceCommand command)
    {
        var device = await deviceRepository.FindBySerialNumberAsync(command.serial);
        if (device is null)
        {
            throw new Exception("Device not found");
        }

        device.Activate();

        deviceRepository.Update(device);
        await uow.CompleteAsync();
    }

    public async Task Handle(DeactivateDeviceCommand command)
    {
        var device = await deviceRepository.FindBySerialNumberAsync(command.serial);
        if (device is null)
        {
            throw new Exception("Device not found");
        }

        device.Deactivate();

        deviceRepository.Update(device);
        await uow.CompleteAsync();
    }
}
