using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Entities;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;

public static class AgronomicThresholdViewResourceFromThresholdAggregateAssembler
{
    public static List<AgronomicThresholdViewResource> ToResourceFromThresholdAggregate(
        List<AgronomicThreshold> thresholds
    )
    {
        return thresholds
            .Select(threshold => new AgronomicThresholdViewResource(
                threshold.EdgeDeviceMacAddress,
                threshold.IotDeviceMacAddress,
                threshold.Min,
                threshold.Max,
                threshold.Description,
                StringFromSensorTypeAssembler.FromSensorTypeToString(threshold.Type)
            ))
            .ToList();
    }
}
