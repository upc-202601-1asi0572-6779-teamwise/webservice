using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Enums;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.ValueObjects;

namespace SmartPalmPlatform.API.CropMonitoring.Domain.Model.Commands;

public record CreatePlantationCommand(
    int PalmGrowerId,
    string Name,
    PlantationLocation Location,
    decimal Hectares,
    CropType CropType
);
