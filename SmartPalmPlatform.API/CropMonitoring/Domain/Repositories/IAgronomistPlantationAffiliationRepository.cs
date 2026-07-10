using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Entities;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;

public interface IAgronomistPlantationAffiliationRepository : IBaseRepository<AgronomistPlantationAffiliation>
{
    Task<List<AgronomistPlantationAffiliation>> FindByAgronomistIdAsync(int agronomistId);
    Task<List<AgronomistPlantationAffiliation>> FindByPlantationIdAsync(int plantationId);
    Task<AgronomistPlantationAffiliation?> FindByAgronomistAndPlantationAsync(int agronomistId, int plantationId);
}