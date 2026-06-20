using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Transform;

public static class RecommendationResourceFromEntityAssembler
{
    public static RecommendationResource ToResourceFromEntity(Recommendation recommendation)
    {
        return new RecommendationResource(
            recommendation.Id,
            recommendation.PlantationId,
            recommendation.AgronomistId,
            recommendation.Content,
            recommendation.Type.ToString(),
            recommendation.Status.ToString(),
            recommendation.CreatedAt,
            recommendation.ApprovedAt,
            recommendation.PublishedAt
        );
    }
}
