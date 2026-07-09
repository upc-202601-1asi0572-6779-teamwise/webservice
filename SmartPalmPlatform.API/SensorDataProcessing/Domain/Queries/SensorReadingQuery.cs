namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;

public record SensorReadingQuery(
    string EdgeDeviceMacAddress,
    DateTime From,
    DateTime To,
    string? IotDeviceMacAddress,
    int Page,
    int Size
);
