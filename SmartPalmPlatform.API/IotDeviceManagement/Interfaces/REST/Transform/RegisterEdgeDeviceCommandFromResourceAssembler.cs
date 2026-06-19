using SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class RegisterEdgeDeviceCommandFromResourceAssembler
{
    public static RegisterEdgeDeviceCommand ToCommandFromResource(
        string edgeDeviceMac,
        int monitoringZoneId,
        EdgeDeviceRegistrationResource resource
    )
    {
        return new RegisterEdgeDeviceCommand(
            edgeDeviceMac,
            monitoringZoneId,
            resource.username,
            resource.password
        );
    }
}
