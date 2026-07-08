namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;

public record DeviceSensorReadingQuery(
    string IotDeviceMacAddress,
    DateTime From,
    DateTime To,
    int Page,
    int Size
);
