using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Repositories;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Application.QueryServices;

public class RecommendationQueryService(IRecommendationRepository recommendationRepository)
    : IRecommendationQueryService
{
    public async Task<IEnumerable<Recommendation>> Handle(GetPlantationRecommendationsQuery query)
    {
        if (query.Status is null && query.AgronomistId is null)
            return await recommendationRepository.FindByPlantationIdAsync(query.PlantationId);

        if (query.Status is not null && query.AgronomistId is not null)
            return await recommendationRepository.FindByPlantationIdAgronomistIdAndStatusAsync(
                query.PlantationId,
                query.AgronomistId.Value,
                query.Status.Value
            );

        if (query.AgronomistId is not null)
            return await recommendationRepository.FindByPlantationIdAndAgronomistIdAsync(
                query.PlantationId,
                query.AgronomistId.Value
            );

        return await recommendationRepository.FindByPlantationIdAndStatusAsync(
            query.PlantationId,
            query.Status!.Value
        );
    }

    public async Task<Recommendation?> Handle(GetRecommendationByIdQuery query)
    {
        return await recommendationRepository.FindByIdAsync(query.RecommendationId);
    }
}

