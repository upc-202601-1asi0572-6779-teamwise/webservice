using SmartPalmPlatform.API.SensorDataProcessing.Domain.Commands;
using SmartPalmPlatform.API.Shared.Domain.Events;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.EventHandlers.Transform;

public static class ReadDeviceSensorsDataCommandFromIotDeviceSynchronizationEventAssembly
{
    public static ReadDeviceSensorsDataCommand FromIotDeviceSynchronizationEventToCommand(
        IotDeviceSynchronizationEvent notification
    )
    {
        // IotDeviceSynchronizationEvent no trae un timestamp propio de sincronización
        // (su origen, EdgeSynchronizationCommand, tampoco lo tiene todavía); se usa
        // DateTime.Now, igual que EdgeDevice.SynchronizeEdgeData() se autotimestampea.
        var command = new ReadDeviceSensorsDataCommand(
            notification.EdgeDeviceMacAddress,
            notification.SynchronizationReadings,
            DateTime.Now
        );

        return command;
    }
}
