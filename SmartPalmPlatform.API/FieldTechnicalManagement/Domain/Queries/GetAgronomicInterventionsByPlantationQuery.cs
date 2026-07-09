namespace SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Queries;

public record GetAgronomicInterventionsByPlantationQuery(
    int PlantationId,
    DateTime? StartDate = null,
    DateTime? EndDate = null
);
