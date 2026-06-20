using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Enums;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Aggregates;

public class SensorReading
{
    public int Id { get; set; } = 0;
    public string EdgeDeviceMacAddress { get; set; } = string.Empty;
    public DateTime MeasuredAt { get; set; } = DateTime.Now;
    public SensorType Type { get; set; } = SensorType.Humidity;
    public MeasureUnit Unit { get; set; } = MeasureUnit.Unknown;
    public double Value { get; set; } = 0;

    public SensorReading() { }

    public SensorReading(
        string edgeDeviceMacAddress,
        DateTime measuredAt,
        SensorType type,
        MeasureUnit unit,
        double value
    )
    {
        this.EdgeDeviceMacAddress = edgeDeviceMacAddress;
        this.MeasuredAt = measuredAt;
        this.Type = type;
        this.Unit = unit;
        this.Value = value;
    }
}
