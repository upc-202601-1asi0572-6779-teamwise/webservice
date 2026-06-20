namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Commands;

public record UpdateRecommendationContentCommand(
    int RecommendationId,
    string Content
);