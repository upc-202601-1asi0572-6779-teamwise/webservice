using SmartPalmPlatform.API.CropMonitoring.Domain.Model.ValueObjects;

namespace SmartPalmPlatform.API.CropMonitoring.Domain.Model.Commands;

public record UpdatePlantationDetailsCommand(
    int Id,
    string Name,
    PlantationLocation Location,
    decimal Hectares
);
