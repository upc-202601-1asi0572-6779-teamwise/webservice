using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Repositories;

public interface IAgronomicInterventionRepository : IBaseRepository<AgronomicIntervention>
{
    Task<IEnumerable<AgronomicIntervention>> FindBySectorIdAsync(
        int sectorId,
        DateTime? startDate = null,
        DateTime? endDate = null
    );
    Task<IEnumerable<AgronomicIntervention>> FindByPlantationIdAsync(
        int plantationId,
        DateTime? startDate = null,
        DateTime? endDate = null
    );
    Task<IEnumerable<AgronomicIntervention>> FindByRecommendationIdAsync(int recommendationId);
}
