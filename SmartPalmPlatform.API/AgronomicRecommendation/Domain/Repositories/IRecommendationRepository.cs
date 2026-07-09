using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Repositories;

public interface IRecommendationRepository : IBaseRepository<Recommendation>
{
    Task<IEnumerable<Recommendation>> FindPendingAsync();

    Task<IEnumerable<Recommendation>> FindByPlantationIdAsync(int plantationId);

    Task<IEnumerable<Recommendation>> FindByPlantationIdAndStatusAsync(
        int plantationId,
        RecommendationStatus status
    );

    Task<IEnumerable<Recommendation>> FindByPlantationIdAndAgronomistIdAsync(
        int plantationId,
        int agronomistId
    );

    Task<IEnumerable<Recommendation>> FindByPlantationIdAgronomistIdAndStatusAsync(
        int plantationId,
        int agronomistId,
        RecommendationStatus status
    );

    Task<IEnumerable<Recommendation>> FindByAgronomistIdAsync(int agronomistId);
}

