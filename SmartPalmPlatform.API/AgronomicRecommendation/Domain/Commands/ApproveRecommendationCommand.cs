namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;

public record ApproveRecommendationCommand(
    int AgronomistId,
    int PlantationId,
    int RecommendationId
);

