using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Entities;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Factory;

public static class AgronomicThresholdTypeFactory
{
    public static AgronomicThreshold DefaultThreshold(
        string edgeDeviceMacAddress,
        string iotDeviceMacAddress,
        SensorType type
    )
    {
        switch (type)
        {
            case SensorType.Humidity:
                return HumiditySensorThreshold(edgeDeviceMacAddress, iotDeviceMacAddress);
            case SensorType.PH:
                return PHSensorThreshold(edgeDeviceMacAddress, iotDeviceMacAddress);
            case SensorType.Luminosity:
                return LuminositySensorThreshold(edgeDeviceMacAddress, iotDeviceMacAddress);
            case SensorType.Temperature:
                return TemperatureSensorThreshold(edgeDeviceMacAddress, iotDeviceMacAddress);
            case SensorType.SoilMoisture:
                return SoilMoistureSensorThreshold(edgeDeviceMacAddress, iotDeviceMacAddress);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public static AgronomicThreshold HumiditySensorThreshold(
        string edgeDeviceMacAddress,
        string iotDeviceMacAddress,
        double min = 0,
        double max = 100,
        string description = "Humidity Sensor Threshold"
    )
    {
        return new AgronomicThreshold(
            edgeDeviceMacAddress,
            iotDeviceMacAddress,
            min,
            max,
            description,
            SensorType.Humidity
        );
    }

    public static AgronomicThreshold PHSensorThreshold(
        string edgeDeviceMacAddress,
        string iotDeviceMacAddress,
        double min = 0,
        double max = 100,
        string description = "PH Sensor Threshold"
    )
    {
        return new AgronomicThreshold(
            edgeDeviceMacAddress,
            iotDeviceMacAddress,
            min,
            max,
            description,
            SensorType.PH
        );
    }

    public static AgronomicThreshold LuminositySensorThreshold(
        string edgeDeviceMacAddress,
        string iotDeviceMacAddress,
        double min = 0,
        double max = 100,
        string description = "Luminosity Sensor Threshold"
    )
    {
        return new AgronomicThreshold(
            edgeDeviceMacAddress,
            iotDeviceMacAddress,
            min,
            max,
            description,
            SensorType.Luminosity
        );
    }

    public static AgronomicThreshold TemperatureSensorThreshold(
        string edgeDeviceMacAddress,
        string iotDeviceMacAddress,
        double min = 10,
        double max = 40,
        string description = "Temperature Sensor Threshold"
    )
    {
        return new AgronomicThreshold(
            edgeDeviceMacAddress,
            iotDeviceMacAddress,
            min,
            max,
            description,
            SensorType.Temperature
        );
    }

    public static AgronomicThreshold SoilMoistureSensorThreshold(
        string edgeDeviceMacAddress,
        string iotDeviceMacAddress,
        double min = 20,
        double max = 80,
        string description = "Soil Moisture Sensor Threshold"
    )
    {
        return new AgronomicThreshold(
            edgeDeviceMacAddress,
            iotDeviceMacAddress,
            min,
            max,
            description,
            SensorType.SoilMoisture
        );
    }
}
