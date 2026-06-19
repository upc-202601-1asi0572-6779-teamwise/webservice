using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;
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
}