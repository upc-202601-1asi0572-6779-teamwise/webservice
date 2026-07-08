namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;

public record CreatePlantationResource(
    string name,
    decimal hectares,
    string address,
    string? coordinates,
    string cropType
);
