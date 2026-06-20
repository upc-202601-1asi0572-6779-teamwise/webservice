using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;

public record GetAgronomistPlantationRecommendationsByStatusQuery(
    int AgronomistId,
    int PlantationId,
    RecommendationStatus? Status
);
