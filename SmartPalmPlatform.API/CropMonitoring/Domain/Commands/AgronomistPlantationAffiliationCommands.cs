namespace SmartPalmPlatform.API.CropMonitoring.Domain.Commands;

public record CreateAgronomistPlantationAffiliationCommand(
    int AgronomistId,
    int PlantationId
);

public record DetachAgronomistPlantationAffiliationCommand(
    int AgronomistId,
    int PlantationId
);