namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;

public record GetAgronomistPlantationRecomendationByIdQuery(
    int AgronomistId,
    int PlantationId,
    int RecommendationId
);
