namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

public record RegisterAgronomicInterventionResource(
    string description,
    string performedBy,
    DateTime executionDate
);