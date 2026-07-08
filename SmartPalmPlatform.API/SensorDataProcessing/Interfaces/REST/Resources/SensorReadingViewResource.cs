namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;

public record SensorReadingViewResource(
    int Id,
    string EdgeDeviceMacAddress,
    string IotDeviceMacAddress,
    string SensorType,
    double Value,
    string Unit,
    DateTime MeasuredAt
);
