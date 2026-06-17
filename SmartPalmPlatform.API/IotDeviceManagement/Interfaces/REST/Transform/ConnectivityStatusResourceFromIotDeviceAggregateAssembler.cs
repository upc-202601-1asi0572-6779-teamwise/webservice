using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class ConnectivityStatusResourceFromIotDeviceAggregateAssembler
{
    public static ConnectivityStatusResource ToResourceFromIotDeviceAggregate(IotDevice device)
    {
        return new ConnectivityStatusResource(
            device.SerialNumber,
            device.IsConnected,
            device.ConnectivityStatus.ToString()
        );
    }
}
