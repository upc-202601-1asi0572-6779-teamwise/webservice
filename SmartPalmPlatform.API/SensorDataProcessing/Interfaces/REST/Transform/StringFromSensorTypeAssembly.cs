using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;

public static class StringFromSensorTypeAssembler
{
    public static string FromSensorTypeToString(SensorType sensorType)
    {
        return sensorType switch
        {
            SensorType.Humidity => "Humidity",
            SensorType.PH => "PH",
            SensorType.Luminosity => "Luminosity",
            _ => "Unknown",
        };
    }
}
