using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;

namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Transform;

public static class StringFromSensorTypeAssembler
{
    public static string FromSensorTypeToString(SensorType sensorType)
    {
        return sensorType switch
        {
            SensorType.Temperature => "Temperature",
            SensorType.Humidity => "Humidity",
            SensorType.Pressure => "Pressure",
            SensorType.Luminosity => "Luminosity",
            SensorType.GasResistance => "GasResistance",
            SensorType.Voltage => "Voltage",
            SensorType.Current => "Current",
            SensorType.Power => "Power",
            SensorType.Speed => "Speed",
            SensorType.Direction => "Direction",
            _ => "Unknown",
        };
    }
}
