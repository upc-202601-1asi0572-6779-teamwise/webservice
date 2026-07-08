namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;

public record PlantationResource(
    string name,
    string address,
    string? coordinates,
    decimal hectares,
    string cropType,
    string status,
    int estimatedSensors,
    string installationMessage,
    DateTime createdAt,
    DateTime? updatedAt
);
