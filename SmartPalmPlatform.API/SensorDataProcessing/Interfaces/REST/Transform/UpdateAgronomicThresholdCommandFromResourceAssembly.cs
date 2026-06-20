using SmartPalmPlatform.API.SensorDataProcessing.Domain.Commands;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;

public static class UpdateAgronomicThresholdCommandFromResourceAssembly
{
    public static UpdateAgronomicThresholdCommand FromResourceToCommand(
        string edgeMac,
        string iotMac,
        UpdateAgronomicThresholdResource resource
    )
    {
        var command = new UpdateAgronomicThresholdCommand(
            edgeMac,
            iotMac,
            SensorTypeFromStringAssembler.FromStringToSensorType(resource.type),
            resource.min,
            resource.max,
            resource.description
        );

        return command;
    }
}
