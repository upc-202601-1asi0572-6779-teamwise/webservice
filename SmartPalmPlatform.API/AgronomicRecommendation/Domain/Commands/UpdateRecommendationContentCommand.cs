namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;

public record UpdateRecommendationContentCommand(
    int AgronomistId,
    int PlantationId,
    int RecommendationId,
    string Content
);

