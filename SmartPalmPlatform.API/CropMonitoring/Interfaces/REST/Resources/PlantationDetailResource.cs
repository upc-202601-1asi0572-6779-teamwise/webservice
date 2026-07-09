namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;

public record SectorResource(
    int id,
    string name,
    string iotDeviceMacAddress,
    string status,
    DateTime createdAt,
    DateTime? activatedAt
);

public record PlantationDetailResource(
    int id,
    string name,
    string address,
    string? coordinates,
    decimal hectares,
    string status,
    int estimatedSensors,
    string installationMessage,
    DateTime createdAt,
    DateTime? updatedAt,
    List<SectorResource> sectors
);
