namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;

public record PlantationResource(
    int id,
    string name,
    string address,
    string? coordinates,
    decimal hectares,
    string status,
    int estimatedSensors,
    string installationMessage,
    DateTime createdAt,
    DateTime? updatedAt
);
