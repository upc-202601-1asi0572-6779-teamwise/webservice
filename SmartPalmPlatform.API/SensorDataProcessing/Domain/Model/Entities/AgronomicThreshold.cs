using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Entities;

public class AgronomicThreshold
{
    public int Id { get; set; } = 0;
    public string EdgeDeviceMacAddress { get; set; } = string.Empty;
    public string IotDeviceMacAddress { get; set; } = string.Empty;
    public double Min { get; set; } = 0;
    public double Max { get; set; } = 0;
    public string Description { get; set; } = String.Empty;
    public SensorType Type { get; set; } = SensorType.Humidity;

    public AgronomicThreshold() { }

    public AgronomicThreshold(
        string edgeDeviceMacAddress,
        string iotDeviceMacAddress,
        double min,
        double max,
        string description,
        SensorType type
    )
    {
        this.EdgeDeviceMacAddress = edgeDeviceMacAddress;
        this.IotDeviceMacAddress = iotDeviceMacAddress;
        this.Min = min;
        this.Max = max;
        this.Description = description;
        this.Type = type;
    }

    public bool IsExceededBy(double value)
    {
        return value > this.Max || value < this.Min;
    }

    public bool IsThresholdSet()
    {
        return this.Min != this.Max;
    }

    public bool Update(double? min, double? max, string? description)
    {
        bool wasUpdated = false;

        if (min is not null)
        {
            this.Min = min.Value;
            wasUpdated = true;
        }

        if (max is not null)
        {
            this.Max = max.Value;
            wasUpdated = true;
        }

        if (description is not null)
        {
            this.Description = description;
            wasUpdated = true;
        }

        return wasUpdated;
    }
}
