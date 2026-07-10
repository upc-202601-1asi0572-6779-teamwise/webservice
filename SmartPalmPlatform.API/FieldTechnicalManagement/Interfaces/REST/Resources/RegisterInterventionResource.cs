namespace SmartPalmPlatform.API.FieldTechnicalManagement.Interfaces.REST.Resources;

public record RegisterInterventionResource(
    string description,
    string performedBy,
    DateTime executionDate,
    int? originRecommendationId
);
