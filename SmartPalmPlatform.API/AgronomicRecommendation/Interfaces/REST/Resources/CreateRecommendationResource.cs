namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

public record CreateRecommendationResource(
    int plantationId,
    int agronomistId,
    string content
);