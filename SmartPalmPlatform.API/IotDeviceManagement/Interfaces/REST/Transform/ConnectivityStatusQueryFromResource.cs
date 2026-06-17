using SmartPalmPlatform.API.IotDeviceManagement.Domain.Queries;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class ConnectiviyStatusQueryFromResourceAssembler
{
    public static ConnectiviyStatusQuery ToQueryFromResource(string serial)
    {
        return new ConnectiviyStatusQuery(serial);
    }
}
