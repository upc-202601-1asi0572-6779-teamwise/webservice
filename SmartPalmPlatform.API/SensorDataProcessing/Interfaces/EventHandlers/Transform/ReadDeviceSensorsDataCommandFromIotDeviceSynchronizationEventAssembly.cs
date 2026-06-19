using SmartPalmPlatform.API.SensorDataProcessing.Domain.Commands;
using SmartPalmPlatform.API.Shared.Domain.Events;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.EventHandlers.Transform;

public static class ReadDeviceSensorsDataCommandFromIotDeviceSynchronizationEventAssembly
{
    public static ReadDeviceSensorsDataCommand FromIotDeviceSynchronizationEventToCommand(
        IotDeviceSynchronizationEvent notification
    )
    {
        var command = new ReadDeviceSensorsDataCommand(
            notification.EdgeDeviceMacAddress,
            notification.SynchronizationReadings
        );

        return command;
    }
}
