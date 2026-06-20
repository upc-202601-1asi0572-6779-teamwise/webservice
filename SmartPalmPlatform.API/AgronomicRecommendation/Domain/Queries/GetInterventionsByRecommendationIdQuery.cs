namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;

public record GetInterventionsByRecommendationIdQuery(
    int AgronomistId,
    int PlantationId,
    int RecommendationId
);

