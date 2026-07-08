using SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;
using SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class RegisterEdgeDeviceCommandFromResourceAssembler
{
    public static RegisterEdgeDeviceCommand ToCommandFromResource(
        EdgeDeviceRegistrationResource resource
    )
    {
        return new RegisterEdgeDeviceCommand(
            resource.edgeMac,
            resource.monitoringZoneId,
            resource.userId
        );
    }
}
