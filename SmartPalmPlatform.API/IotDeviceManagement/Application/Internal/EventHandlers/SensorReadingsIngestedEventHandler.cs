using SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Application.Internal.EventHandlers;
using SmartPalmPlatform.API.Shared.Domain.Events;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.IotDeviceManagement.Application.Internal.EventHandlers;

public class SensorReadingsIngestedEventHandler(
    IUnitOfWork uow,
    IEdgeDeviceRepository edgeDeviceRepository
) : IEventHandler<SensorReadingsIngestedEvent>
{
    public async Task Handle(
        SensorReadingsIngestedEvent notification,
        CancellationToken cancellationToken
    )
    {
        var device = await edgeDeviceRepository.FindByMacAddress(
            notification.EdgeDeviceMacAddress
        );
        if (device is null)
            return;

        device.SynchronizeEdgeData();
        edgeDeviceRepository.Update(device);
        await uow.CompleteAsync();
    }
}
