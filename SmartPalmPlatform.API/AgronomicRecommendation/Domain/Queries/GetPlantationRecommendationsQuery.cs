using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;

public record GetSectorRecommendationsQuery(
    int SectorId,
    RecommendationStatus? Status = null,
    int? AgronomistId = null
);

public record GetGeneralRecommendationsQuery(
    RecommendationStatus? Status = null,
    int? AgronomistId = null
);
