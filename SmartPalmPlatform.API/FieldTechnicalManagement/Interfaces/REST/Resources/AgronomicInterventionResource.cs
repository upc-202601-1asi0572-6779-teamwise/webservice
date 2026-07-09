namespace SmartPalmPlatform.API.FieldTechnicalManagement.Interfaces.REST.Resources;

public record AgronomicInterventionResource(
    int id,
    int sectorId,
    string performedBy,
    string description,
    DateTime executionDate,
    DateTime createdAt,
    int? recommendationId
);
