using SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class RegisterDeviceCommandFromResourceAssembler
{
    public static RegisterDeviceCommand ToCommandFromResource(DeviceRegistrationResource resource)
    {
        return new RegisterDeviceCommand(
            resource.serialNumber,
            resource.monitoringZoneId,
            resource.username,
            resource.password
        );
    }
}
