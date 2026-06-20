namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

public record RecommendationResource(
    int id,
    int plantationId,
    int agronomistId,
    string content,
    string type,
    string status,
    DateTime createdAt,
    DateTime? approvedAt,
    DateTime? publishedAt
);