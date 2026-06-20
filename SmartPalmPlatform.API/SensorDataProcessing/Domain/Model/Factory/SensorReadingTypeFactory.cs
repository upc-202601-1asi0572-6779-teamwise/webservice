using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Aggregates;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Enums;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Factory;

public static class SensorReadingTypeFactory
{
    public static SensorReading DefaultSensorReading(
        string edgeDeviceMacAddress,
        SensorType type,
        DateTime measuredAt,
        double value
    )
    {
        switch (type)
        {
            case SensorType.Humidity:
                return HumiditySensorReading(edgeDeviceMacAddress, measuredAt, value);
            case SensorType.PH:
                return PHSensorReading(edgeDeviceMacAddress, measuredAt, value);
            case SensorType.Luminosity:
                return LuminositySensorReading(edgeDeviceMacAddress, measuredAt, value);
            case SensorType.Temperature:
                return TemperatureSensorReading(edgeDeviceMacAddress, measuredAt, value);
            case SensorType.SoilMoisture:
                return SoilMoistureSensorReading(edgeDeviceMacAddress, measuredAt, value);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public static SensorReading HumiditySensorReading(
        string edgeDeviceMacAddress,
        DateTime measuredAt,
        double value
    )
    {
        return new SensorReading(
            edgeDeviceMacAddress,
            measuredAt,
            SensorType.Humidity,
            MeasureUnit.Percent,
            value
        );
    }

    public static SensorReading PHSensorReading(
        string edgeDeviceMacAddress,
        DateTime measuredAt,
        double value
    )
    {
        return new SensorReading(
            edgeDeviceMacAddress,
            measuredAt,
            SensorType.PH,
            MeasureUnit.Unknown,
            value
        );
    }

    public static SensorReading LuminositySensorReading(
        string edgeDeviceMacAddress,
        DateTime measuredAt,
        double value
    )
    {
        return new SensorReading(
            edgeDeviceMacAddress,
            measuredAt,
            SensorType.Luminosity,
            MeasureUnit.Unknown,
            value
        );
    }

    public static SensorReading TemperatureSensorReading(
        string edgeDeviceMacAddress,
        DateTime measuredAt,
        double value
    )
    {
        return new SensorReading(
            edgeDeviceMacAddress,
            measuredAt,
            SensorType.Temperature,
            MeasureUnit.Unknown,
            value
        );
    }

    public static SensorReading SoilMoistureSensorReading(
        string edgeDeviceMacAddress,
        DateTime measuredAt,
        double value
    )
    {
        return new SensorReading(
            edgeDeviceMacAddress,
            measuredAt,
            SensorType.SoilMoisture,
            MeasureUnit.Percent,
            value
        );
    }
}
