using SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class DeactivateDeviceCommandFromResourceAssembler
{
    public static DeactivateDeviceCommand ToCommandFromResource(string serial)
    {
        return new DeactivateDeviceCommand(serial);
    }
}
