using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Entities;
using SmartPalmPlatform.API.CropMonitoring.Domain.Queries;
using SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services;

namespace SmartPalmPlatform.API.CropMonitoring.Application;

public class AgronomistPlantationAffiliationQueryService(
    IAgronomistPlantationAffiliationRepository affiliationRepository
) : IAgronomistPlantationAffiliationQueryService
{
    public async Task<List<AgronomistPlantationAffiliation>> Handle(GetAgronomistPlantationAffiliationsQuery query)
    {
        return await affiliationRepository.FindByAgronomistIdAsync(query.AgronomistId);
    }

    public async Task<List<AgronomistPlantationAffiliation>> Handle(GetPlantationAgronomistAffiliationsQuery query)
    {
        return await affiliationRepository.FindByPlantationIdAsync(query.PlantationId);
    }
}