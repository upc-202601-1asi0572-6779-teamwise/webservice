namespace SmartPalmPlatform.API.FieldTechnicalManagement.Interfaces.REST.Resources;

public record RegisterInterventionResource(
    int sectorId,
    string description,
    string performedBy,
    DateTime executionDate,
    int? originRecommendationId
);
