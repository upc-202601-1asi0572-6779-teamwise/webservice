using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class ConnectivityStatusResourceFromEdgeDeviceAggregateAssembler
{
    public static ConnectivityStatusResource ToResourceFromEdgeDeviceAggregate(EdgeDevice device)
    {
        return new ConnectivityStatusResource(
            device.MacAddress,
            device.IsConnected,
            StringFromConnectivityStatus(device.IsConnected)
        );
    }

    private static string StringFromConnectivityStatus(bool isConnected)
    {
        return isConnected ? "Connected" : "Disconnected";
    }
}
