namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;

public record UpdatePlantationResource(
    string name,
    decimal hectares,
    string address,
    string? coordinates
);
