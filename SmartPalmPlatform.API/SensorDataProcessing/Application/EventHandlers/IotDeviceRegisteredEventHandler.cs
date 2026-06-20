using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Factory;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Application.Internal.EventHandlers;
using SmartPalmPlatform.API.Shared.Domain.Events;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.SensorDataProcessing.Application.EventHandlers;

public class IotDeviceRegisteredEventHandler(
    IUnitOfWork uow,
    IAgronomicThresholdRepository agronomicThresholdRepository
) : IEventHandler<IotDeviceRegisteredEvent>
{
    public async Task Handle(
        IotDeviceRegisteredEvent notification,
        CancellationToken cancellationToken
    )
    {
        foreach (var type in Enum.GetValues<SensorType>())
        {
            var threshold = AgronomicThresholdTypeFactory.DefaultThreshold(
                notification.EdgeDeviceMacAddress,
                notification.IotDeviceMacAddress,
                type
            );
            await agronomicThresholdRepository.AddAsync(threshold);
            await uow.CompleteAsync();
        }
    }
}
