using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class ActivationStatusResourceFromIotDeviceAggregateAssembler
{
    public static ActivationStatusResource ToResourceFromIotDeviceAggregate(IotDevice device)
    {
        return new ActivationStatusResource(device.SerialNumber, device.IsActive);
    }
}
