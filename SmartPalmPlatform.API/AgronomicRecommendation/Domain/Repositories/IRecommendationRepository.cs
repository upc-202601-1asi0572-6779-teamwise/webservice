using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Repositories;

public interface IRecommendationRepository : IBaseRepository<Recommendation>
{
    Task<IEnumerable<Recommendation>> FindPendingAsync();

    Task<IEnumerable<Recommendation>> FindBySectorIdAsync(int sectorId);

    Task<IEnumerable<Recommendation>> FindBySectorIdAndStatusAsync(
        int sectorId,
        RecommendationStatus status
    );

    Task<IEnumerable<Recommendation>> FindBySectorIdAndAgronomistIdAsync(
        int sectorId,
        int agronomistId
    );

    Task<IEnumerable<Recommendation>> FindBySectorIdAgronomistIdAndStatusAsync(
        int sectorId,
        int agronomistId,
        RecommendationStatus status
    );

    Task<IEnumerable<Recommendation>> FindByAgronomistIdAsync(int agronomistId);

    Task<IEnumerable<Recommendation>> FindGeneralAsync();

    Task<IEnumerable<Recommendation>> FindGeneralAndStatusAsync(RecommendationStatus status);

    Task<IEnumerable<Recommendation>> FindByReportIdAsync(int reportId);
}

