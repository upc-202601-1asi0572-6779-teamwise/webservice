namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;

public record SensorReadingQuery(string EdgeDeviceMacAddress, DateTime From, DateTime To);
