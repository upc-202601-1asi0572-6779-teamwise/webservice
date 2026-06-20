namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;

public record PublishRecommendationCommand(
    int AgronomistId,
    int PlantationId,
    int RecommendationId
);

