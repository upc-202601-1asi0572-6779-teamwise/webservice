namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;

public record ReadDeviceSensorsDataResource(List<SensorDataResource> readings, DateTime measuredAt);
