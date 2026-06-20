using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Repositories;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Application.QueryServices;

public class RecommendationQueryService(IRecommendationRepository recommendationRepository)
    : IRecommendationQueryService
{
    public async Task<IEnumerable<Recommendation>> Handle(
        GetAgronomistPlantationRecommendationsQuery query
    )
    {
        // TODO: validate if agronomist exists and if plantation exists
        return await recommendationRepository.FindByAgronomistIdAndPlantationIdAsync(
            query.AgronomistId,
            query.PlantationId
        );
    }

    public async Task<Recommendation?> Handle(GetAgronomistPlantationRecomendationByIdQuery query)
    {
        // TODO: validate if agronomist exists, if plantation exists and if recommendation exists
        return await recommendationRepository.FindByIdAsync(query.RecommendationId);
    }

    public async Task<IEnumerable<Recommendation>> Handle(
        GetAgronomistPlantationRecommendationsByStatusQuery query
    )
    {
        // TODO: validate if agronomist exists and if plantation exists

        if (query.Status is null)
            return await recommendationRepository.FindByAgronomistIdAndPlantationIdAsync(
                query.AgronomistId,
                query.PlantationId
            );

        return await recommendationRepository.FindByAgronomistIdPlantationIdAndStatusAsync(
            query.AgronomistId,
            query.PlantationId,
            query.Status.Value
        );
    }

    public async Task<IEnumerable<AgronomicIntervention>> Handle(
        GetInterventionsByRecommendationIdQuery query
    )
    {
        // TODO: validate if agronomist exists, if plantation exists and if recommendation exists
        return await recommendationRepository.FindInterventionsByRecommendationIdAsync(
            query.RecommendationId
        );
    }
}

