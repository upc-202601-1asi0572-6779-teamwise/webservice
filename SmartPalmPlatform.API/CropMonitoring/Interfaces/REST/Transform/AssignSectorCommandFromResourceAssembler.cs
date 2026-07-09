using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Commands;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Transform;

public static class AssignSectorCommandFromResourceAssembler
{
    public static AssignSectorCommand ToCommandFromResource(
        int plantationId,
        AssignSectorResource resource
    )
    {
        return new AssignSectorCommand(
            plantationId,
            resource.iotDeviceMacAddress,
            resource.sectorName
        );
    }
}
