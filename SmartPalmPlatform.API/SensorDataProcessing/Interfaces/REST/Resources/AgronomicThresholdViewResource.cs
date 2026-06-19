namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;

public record AgronomicThresholdViewResource(
    string edgeMac,
    string iotMac,
    double min,
    double max,
    string description,
    string type
);
