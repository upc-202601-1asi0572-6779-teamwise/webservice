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
}
