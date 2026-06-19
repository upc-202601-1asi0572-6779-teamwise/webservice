using SmartPalmPlatform.API.IotDeviceManagement.Domain.Queries;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class EdgeRegistryQueryFromResourceAssembler
{
    public static EdgeRegistryQuery ToQueryFromResource(string edgeMac)
    {
        return new EdgeRegistryQuery(edgeMac);
    }
}
