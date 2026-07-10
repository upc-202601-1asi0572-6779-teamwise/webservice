using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Queries;

namespace SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Services;

public interface IAgronomicInterventionQueryService
{
    Task<AgronomicIntervention?> Handle(GetAgronomicInterventionByIdQuery query);
    Task<IEnumerable<AgronomicIntervention>> Handle(GetAgronomicInterventionsBySectorIdQuery query);
    Task<IEnumerable<AgronomicIntervention>> Handle(GetAgronomicInterventionsByPlantationQuery query);
    Task<IEnumerable<AgronomicIntervention>> Handle(GetAgronomicInterventionsByRecommendationIdQuery query);
}
