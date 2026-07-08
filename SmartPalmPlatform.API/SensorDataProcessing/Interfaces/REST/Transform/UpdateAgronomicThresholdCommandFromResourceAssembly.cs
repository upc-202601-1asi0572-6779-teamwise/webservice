using SmartPalmPlatform.API.SensorDataProcessing.Domain.Commands;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;

public static class UpdateAgronomicThresholdCommandFromResourceAssembly
{
    public static UpdateAgronomicThresholdCommand FromResourceToCommand(
        string deviceMac,
        UpdateAgronomicThresholdResource resource
    )
    {
        var command = new UpdateAgronomicThresholdCommand(
            deviceMac,
            SensorTypeFromStringAssembler.FromStringToSensorType(resource.type),
            resource.min,
            resource.max,
            resource.description
        );

        return command;
    }
}
