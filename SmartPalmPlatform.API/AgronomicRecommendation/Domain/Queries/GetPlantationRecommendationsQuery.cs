using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;

public record GetPlantationRecommendationsQuery(
    int PlantationId,
    RecommendationStatus? Status = null,
    int? AgronomistId = null
);
