using SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.IotDeviceManagement.Application.CommandServices;

public class EdgeSyncrhonizationService(IUnitOfWork uow, IIotDeviceRepository deviceRepository)
    : IEdgeSynchronizationService
{
    public async Task Handle(EdgeSynchronizationCommand command)
    {
        var device = await deviceRepository.FindBySerialNumberAsync(command.serial);
        if (device is null)
        {
            throw new Exception("Device not found");
        }

        device.SynchronizeEdgeData();

        deviceRepository.Update(device);
        await uow.CompleteAsync();
    }
}
