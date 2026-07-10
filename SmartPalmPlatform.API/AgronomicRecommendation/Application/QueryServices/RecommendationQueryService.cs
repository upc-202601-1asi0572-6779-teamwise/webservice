using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Repositories;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Application.QueryServices;

public class RecommendationQueryService(IRecommendationRepository recommendationRepository)
    : IRecommendationQueryService
{
    public async Task<IEnumerable<Recommendation>> Handle(GetSectorRecommendationsQuery query)
    {
        if (query.Status is null && query.AgronomistId is null)
            return await recommendationRepository.FindBySectorIdAsync(query.SectorId);

        if (query.Status is not null && query.AgronomistId is not null)
            return await recommendationRepository.FindBySectorIdAgronomistIdAndStatusAsync(
                query.SectorId,
                query.AgronomistId.Value,
                query.Status.Value
            );

        if (query.AgronomistId is not null)
            return await recommendationRepository.FindBySectorIdAndAgronomistIdAsync(
                query.SectorId,
                query.AgronomistId.Value
            );

        return await recommendationRepository.FindBySectorIdAndStatusAsync(
            query.SectorId,
            query.Status!.Value
        );
    }

    public async Task<IEnumerable<Recommendation>> Handle(GetGeneralRecommendationsQuery query)
    {
        if (query.Status is not null)
            return await recommendationRepository.FindGeneralAndStatusAsync(query.Status.Value);

        return await recommendationRepository.FindGeneralAsync();
    }

    public async Task<IEnumerable<Recommendation>> Handle(GetRecommendationsByReportIdQuery query)
    {
        return await recommendationRepository.FindByReportIdAsync(query.ReportId);
    }

    public async Task<Recommendation?> Handle(GetRecommendationByIdQuery query)
    {
        return await recommendationRepository.FindByIdAsync(query.RecommendationId);
    }
}

