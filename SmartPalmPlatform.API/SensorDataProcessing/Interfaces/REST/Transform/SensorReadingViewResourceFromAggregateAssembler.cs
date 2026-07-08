using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Aggregates;
using SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;

public static class SensorReadingViewResourceFromAggregateAssembler
{
    public static SensorReadingViewResource ToResourceFromAggregate(SensorReading reading)
    {
        return new SensorReadingViewResource(
            reading.Id,
            reading.EdgeDeviceMacAddress,
            reading.IotDeviceMacAddress,
            StringFromSensorTypeAssembler.FromSensorTypeToString(reading.Type),
            reading.Value,
            reading.Unit.ToString(),
            reading.MeasuredAt
        );
    }

    public static List<SensorReadingViewResource> ToResourceListFromAggregateList(
        List<SensorReading> readings
    )
    {
        return readings.Select(ToResourceFromAggregate).ToList();
    }
}
