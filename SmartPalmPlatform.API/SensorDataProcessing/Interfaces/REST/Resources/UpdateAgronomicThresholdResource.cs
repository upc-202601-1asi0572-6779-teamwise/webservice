namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Resources;

public record UpdateAgronomicThresholdResource(
    string type,
    double? min,
    double? max,
    string? description
);
