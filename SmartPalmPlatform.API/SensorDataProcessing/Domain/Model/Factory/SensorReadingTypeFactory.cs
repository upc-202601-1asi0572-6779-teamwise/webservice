using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Aggregates;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Enums;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Factory;

public static class SensorReadingTypeFactory
{
    public static SensorReading DefaultSensorReading(
        string edgeDeviceMacAddress,
        string iotDeviceMacAddress,
        SensorType type,
        DateTime measuredAt,
        double value
    )
    {
        switch (type)
        {
            case SensorType.Humidity:
                return HumiditySensorReading(
                    edgeDeviceMacAddress,
                    iotDeviceMacAddress,
                    measuredAt,
                    value
                );
            case SensorType.PH:
                return PHSensorReading(
                    edgeDeviceMacAddress,
                    iotDeviceMacAddress,
                    measuredAt,
                    value
                );
            case SensorType.Luminosity:
                return LuminositySensorReading(
                    edgeDeviceMacAddress,
                    iotDeviceMacAddress,
                    measuredAt,
                    value
                );
            case SensorType.Temperature:
                return TemperatureSensorReading(
                    edgeDeviceMacAddress,
                    iotDeviceMacAddress,
                    measuredAt,
                    value
                );
            case SensorType.SoilMoisture:
                return SoilMoistureSensorReading(
                    edgeDeviceMacAddress,
                    iotDeviceMacAddress,
                    measuredAt,
                    value
                );
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public static SensorReading HumiditySensorReading(
        string edgeDeviceMacAddress,
        string iotDeviceMacAddress,
        DateTime measuredAt,
        double value
    )
    {
        return new SensorReading(
            edgeDeviceMacAddress,
            iotDeviceMacAddress,
            measuredAt,
            SensorType.Humidity,
            MeasureUnit.Percent,
            value
        );
    }

    public static SensorReading PHSensorReading(
        string edgeDeviceMacAddress,
        string iotDeviceMacAddress,
        DateTime measuredAt,
        double value
    )
    {
        return new SensorReading(
            edgeDeviceMacAddress,
            iotDeviceMacAddress,
            measuredAt,
            SensorType.PH,
            MeasureUnit.Unknown,
            value
        );
    }

    public static SensorReading LuminositySensorReading(
        string edgeDeviceMacAddress,
        string iotDeviceMacAddress,
        DateTime measuredAt,
        double value
    )
    {
        return new SensorReading(
            edgeDeviceMacAddress,
            iotDeviceMacAddress,
            measuredAt,
            SensorType.Luminosity,
            MeasureUnit.Unknown,
            value
        );
    }

    public static SensorReading TemperatureSensorReading(
        string edgeDeviceMacAddress,
        string iotDeviceMacAddress,
        DateTime measuredAt,
        double value
    )
    {
        return new SensorReading(
            edgeDeviceMacAddress,
            iotDeviceMacAddress,
            measuredAt,
            SensorType.Temperature,
            MeasureUnit.Unknown,
            value
        );
    }

    public static SensorReading SoilMoistureSensorReading(
        string edgeDeviceMacAddress,
        string iotDeviceMacAddress,
        DateTime measuredAt,
        double value
    )
    {
        return new SensorReading(
            edgeDeviceMacAddress,
            iotDeviceMacAddress,
            measuredAt,
            SensorType.SoilMoisture,
            MeasureUnit.Percent,
            value
        );
    }
}
