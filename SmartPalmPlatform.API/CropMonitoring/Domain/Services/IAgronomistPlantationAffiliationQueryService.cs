using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Entities;
using SmartPalmPlatform.API.CropMonitoring.Domain.Queries;

namespace SmartPalmPlatform.API.CropMonitoring.Domain.Services;

public interface IAgronomistPlantationAffiliationQueryService
{
    Task<List<AgronomistPlantationAffiliation>> Handle(GetAgronomistPlantationAffiliationsQuery query);
    Task<List<AgronomistPlantationAffiliation>> Handle(GetPlantationAgronomistAffiliationsQuery query);
}