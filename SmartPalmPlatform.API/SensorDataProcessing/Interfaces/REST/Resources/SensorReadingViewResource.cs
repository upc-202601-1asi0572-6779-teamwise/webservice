namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;

public record SensorReadingViewResource(
    string edgeDeviceMacAddress,
    string iotDeviceMacAddress,
    string sensorType,
    double value,
    string unit,
    DateTime measuredAt
);
