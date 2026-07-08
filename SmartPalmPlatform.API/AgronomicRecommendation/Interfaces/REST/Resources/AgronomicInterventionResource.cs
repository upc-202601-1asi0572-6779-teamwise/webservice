namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

public record AgronomicInterventionResource(
    string description,
    string performedBy,
    DateTime executionDate,
    DateTime createdAt
);