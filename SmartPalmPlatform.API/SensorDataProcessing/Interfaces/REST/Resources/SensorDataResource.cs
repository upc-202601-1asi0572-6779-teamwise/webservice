namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;

public record SensorDataResource(string sensorType, DateTime measuredAt, double value);
