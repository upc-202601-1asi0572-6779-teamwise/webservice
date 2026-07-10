namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

public record CreateRecommendationResource(int agronomistId, string content, int? reportId = null);

