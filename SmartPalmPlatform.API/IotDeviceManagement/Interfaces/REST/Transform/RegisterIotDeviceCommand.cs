using SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class RegisterIotDeviceCommandFromResourceAssembler
{
    public static RegisterIotDeviceCommand ToCommandFromResource(
        string edgeDeviceMac,
        IotDeviceRegistrationResource resource
    )
    {
        return new RegisterIotDeviceCommand(
            edgeDeviceMac,
            resource.iotMac,
            resource.username,
            resource.password
        );
    }
}
