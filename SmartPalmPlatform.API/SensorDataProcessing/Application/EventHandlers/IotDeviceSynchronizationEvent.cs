using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.CommandServices;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.EventHandlers.Transform;
using SmartPalmPlatform.API.Shared.Application.Internal.EventHandlers;
using SmartPalmPlatform.API.Shared.Domain.Events;

namespace SmartPalmPlatform.API.SensorDataProcessing.Application.EventHandlers;

public class IotDeviceSynchronizationEventHandler(
    ISensorReadingCommandService sensorReadingCommandService
) : IEventHandler<IotDeviceSynchronizationEvent>
{
    public async Task Handle(
        IotDeviceSynchronizationEvent notification,
        CancellationToken cancellationToken
    )
    {
        var command =
            ReadDeviceSensorsDataCommandFromIotDeviceSynchronizationEventAssembly.FromIotDeviceSynchronizationEventToCommand(
                notification
            );

        await sensorReadingCommandService.Handle(command);
    }
}
