namespace SmartPalmPlatform.API.CropMonitoring.Domain.Model.Commands;

public record AssignSectorCommand(
    int PlantationId,
    string IotDeviceMacAddress,
    string SectorName
);
