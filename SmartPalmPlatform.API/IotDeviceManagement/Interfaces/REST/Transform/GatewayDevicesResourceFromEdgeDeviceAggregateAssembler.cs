using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class GatewayDevicesResourceFromEdgeDeviceAggregateAssembler
{
    public static GatewayDevicesResource ToResourceFromEdgeDeviceAggregate(
        EdgeDevice edgeDevice,
        List<EdgeRegistry> registry
    )
    {
        return new GatewayDevicesResource(
            edgeDevice.MacAddress,
            registry
                .Select(entry => new GatewayDeviceResource(entry.IotDeviceMacAddresses))
                .ToList()
        );
    }
}
