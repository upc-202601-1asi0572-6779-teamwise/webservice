using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.Shared.Domain.Model.ValueObjects;

public class SensorReadingPayload(SensorType type, DateTime measuredAt, double value)
{
    public DateTime MeasuredAt { get; set; } = measuredAt;
    public SensorType Type { get; set; } = type;
    public double Value { get; set; } = value;
}
