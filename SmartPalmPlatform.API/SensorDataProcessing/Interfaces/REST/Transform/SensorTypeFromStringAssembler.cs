using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;

public static class SensorTypeFromStringAssembler
{
    public static SensorType FromStringToSensorType(string sensorType)
    {
        return sensorType switch
        {
            "Humidity" => SensorType.Humidity,
            "PH" => SensorType.PH,
            "Luminosity" => SensorType.Luminosity,
            "Temperature" => SensorType.Temperature,
            "SoilMoisture" => SensorType.SoilMoisture,
            _ => throw new ArgumentException($"Unknown sensor type: '{sensorType}'."),
        };
    }
}
