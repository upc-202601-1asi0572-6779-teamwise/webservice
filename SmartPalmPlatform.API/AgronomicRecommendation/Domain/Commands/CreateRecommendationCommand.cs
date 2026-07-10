using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;

public record CreateRecommendationCommand(
    int? SectorId,
    int AgronomistId,
    string Content,
    RecommendationType Type = RecommendationType.SectorSpecific,
    int? ReportId = null
);
