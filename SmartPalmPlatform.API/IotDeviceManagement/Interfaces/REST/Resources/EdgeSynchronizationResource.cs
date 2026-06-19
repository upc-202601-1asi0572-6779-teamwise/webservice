namespace SmartPalmPlatform.API.IotDeviceManagement.Interfaces.REST.Resources;

public record SensorReadingResource(string sensorType, DateTime measuredAt, double value);

public record EdgeSynchronizationResource(List<SensorReadingResource> readings, DateTime syncedAt);
