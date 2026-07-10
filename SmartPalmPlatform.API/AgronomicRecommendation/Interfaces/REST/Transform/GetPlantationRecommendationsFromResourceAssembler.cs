using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Enums;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.REST.Transform;

public static class GetSectorRecommendationsFromResourceAssembler
{
    public static GetSectorRecommendationsQuery ToQueryFromResource(
        int sectorId,
        string? status,
        int? agronomistId = null
    )
    {
        return new GetSectorRecommendationsQuery(
            sectorId,
            status is null ? null : StatusFromString(status),
            agronomistId
        );
    }

    public static GetGeneralRecommendationsQuery ToGeneralQueryFromResource(
        string? status,
        int? agronomistId = null
    )
    {
        return new GetGeneralRecommendationsQuery(
            status is null ? null : StatusFromString(status),
            agronomistId
        );
    }

    private static RecommendationStatus StatusFromString(string status)
    {
        if (Enum.TryParse<RecommendationStatus>(status, ignoreCase: true, out var parsed))
            return parsed;
        throw new ArgumentException($"Invalid status '{status}'. Valid values: Pending, Approved, Published (case-insensitive).");
    }
}
