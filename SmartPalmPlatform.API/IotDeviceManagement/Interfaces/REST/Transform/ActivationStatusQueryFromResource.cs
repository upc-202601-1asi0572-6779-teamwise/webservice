using SmartPalmPlatform.API.IotDeviceManagement.Domain.Queries;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class ActivationStatusQueryFromResourceAssembler
{
    public static ActivationStatusQuery ToQueryFromResource(string serial)
    {
        return new ActivationStatusQuery(serial);
    }
}
