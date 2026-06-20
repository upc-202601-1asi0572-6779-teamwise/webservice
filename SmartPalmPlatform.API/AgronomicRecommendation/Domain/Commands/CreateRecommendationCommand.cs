namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;

public record CreateRecommendationCommand(
    int PlantationId,
    int AgronomistId,
    string Content
);