using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.Shared.Domain.Model.ValueObjects;

public class SensorReadingPayload(
    string iotDeviceMacAddress,
    SensorType type,
    DateTime measuredAt,
    double value
)
{
    public string IotDeviceMacAddress { get; set; } = iotDeviceMacAddress;
    public DateTime MeasuredAt { get; set; } = measuredAt;
    public SensorType Type { get; set; } = type;
    public double Value { get; set; } = value;
}
