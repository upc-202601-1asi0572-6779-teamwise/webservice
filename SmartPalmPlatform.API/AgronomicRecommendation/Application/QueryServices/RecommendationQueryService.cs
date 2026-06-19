using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Repositories;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Application.QueryServices;

public class RecommendationQueryService(IRecommendationRepository recommendationRepository)
    : IRecommendationQueryService
{
    public async Task<IEnumerable<Recommendation>> Handle(GetAllRecommendationsQuery query)
    {
        return await recommendationRepository.ListAsync();
    }

    public async Task<Recommendation?> Handle(GetRecommendationByIdQuery query)
    {
        return await recommendationRepository.FindByIdAsync(query.RecommendationId);
    }

    public async Task<IEnumerable<Recommendation>> Handle(GetPendingRecommendationsQuery query)
    {
        return await recommendationRepository.FindPendingAsync();
    }

    public async Task<IEnumerable<Recommendation>> Handle(
        GetRecommendationsByPlantationIdQuery query
    )
    {
        return await recommendationRepository.FindByPlantationIdAsync(query.PlantationId);
    }

    public async Task<IEnumerable<AgronomicIntervention>> Handle(
        GetInterventionsByRecommendationIdQuery query
    )
    {
        return await recommendationRepository.FindInterventionsByRecommendationIdAsync(
            query.RecommendationId
        );
    }
}