namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;

public record AgronomistPlantationAffiliationResource(
    int Id,
    int AgronomistId,
    int PlantationId,
    DateTime AffiliatedAt,
    DateTime? DetachedAt
);

public record CreateAgronomistPlantationAffiliationResource(
    int AgronomistId,
    int PlantationId
);