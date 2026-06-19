namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;

public class SensorReading(SensorType sensorType, double value, DateTime timestamp)
{
    public int Id { get; private set; }
    public SensorType SensorType { get; private set; } = sensorType;
    public double Value { get; private set; } = value;
    public DateTime Timestamp { get; private set; } = timestamp;

    public SensorReading Clone()
    {
        return new SensorReading(this.SensorType, this.Value, this.Timestamp);
    }
}
