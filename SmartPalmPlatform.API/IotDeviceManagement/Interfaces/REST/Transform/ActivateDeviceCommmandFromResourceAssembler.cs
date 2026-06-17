using SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class ActivateDeviceCommmandFromResourceAssembler
{
    public static ActivateDeviceCommand ToCommandFromResource(string serial)
    {
        return new ActivateDeviceCommand(serial);
    }
}
