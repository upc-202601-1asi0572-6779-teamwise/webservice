namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.ACL;

public interface IRecommendationFacade
{
    Task<bool> RecommendationExistsAndIsPublishedAsync(int recommendationId);
}
