namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

public record AgronomicInterventionResource(
    int id,
    int recommendationId,
    string description,
    string performedBy,
    DateTime executionDate,
    DateTime createdAt
);