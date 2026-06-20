using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Aggregates;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Model.Entities;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Queries;

namespace SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;

public interface IRecommendationQueryService
{
    Task<IEnumerable<Recommendation>> Handle(GetAgronomistPlantationRecommendationsQuery query);

    Task<Recommendation?> Handle(GetAgronomistPlantationRecomendationByIdQuery query);

    Task<IEnumerable<Recommendation>> Handle(
        GetAgronomistPlantationRecommendationsByStatusQuery query
    );

    Task<IEnumerable<AgronomicIntervention>> Handle(GetInterventionsByRecommendationIdQuery query);
}

