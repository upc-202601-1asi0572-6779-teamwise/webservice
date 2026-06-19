using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class EdgeRegistryResourceFromEdgeDeviceAggregateAssembler
{
    public static EdgeRegistryResource ToResourceFromEdgeDeviceAggregate(
        EdgeDevice edgeDevice,
        List<EdgeRegistry> registry
    )
    {
        return new EdgeRegistryResource(
            edgeDevice.MacAddress,
            registry
                .Select(registry => new RegistryIotDeviceResource(registry.IotDeviceMacAddresses))
                .ToList()
        );
    }
}
