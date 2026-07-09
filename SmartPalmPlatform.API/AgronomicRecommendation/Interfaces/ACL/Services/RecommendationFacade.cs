using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.ACL.Services;

public class RecommendationFacade(
    IRecommendationQueryService recommendationQueryService
) : IRecommendationFacade
{
    public async Task<bool> RecommendationExistsAndIsPublishedAsync(int recommendationId)
    {
        var recommendation = await recommendationQueryService.Handle(
            new GetRecommendationByIdQuery(recommendationId)
        );
        return recommendation is not null
            && recommendation.Status == RecommendationStatus.Published;
    }
}
