namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

public record RecommendationResource(
    int id,
    string content,
    string type,
    string status,
    DateTime createdAt,
    DateTime? approvedAt,
    DateTime? publishedAt
);