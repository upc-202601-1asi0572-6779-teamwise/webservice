using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Repositories;

public interface IRecommendationRepository : IBaseRepository<Recommendation>
{
    Task<IEnumerable<Recommendation>> FindPendingAsync();

    Task<IEnumerable<Recommendation>> FindByPlantationIdAsync(int plantationId);

    Task AddInterventionAsync(AgronomicIntervention intervention);

    Task<IEnumerable<AgronomicIntervention>> FindInterventionsByRecommendationIdAsync(
        int recommendationId
    );

    Task<IEnumerable<Recommendation>> FindByAgronomistIdAndPlantationIdAsync(
        int agronomistId,
        int plantationId
    );

    Task<IEnumerable<Recommendation>> FindByAgronomistIdAsync(int agronomistId);

    Task<IEnumerable<Recommendation>> FindByAgronomistIdPlantationIdAndStatusAsync(
        int agronomistId,
        int plantationId,
        RecommendationStatus status
    );
}

