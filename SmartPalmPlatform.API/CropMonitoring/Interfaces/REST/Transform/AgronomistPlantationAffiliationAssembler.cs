using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Entities;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Transform;

public static class AgronomistPlantationAffiliationAssembler
{
    public static AgronomistPlantationAffiliationResource ToResourceFromEntity(
        AgronomistPlantationAffiliation entity)
    {
        return new AgronomistPlantationAffiliationResource(
            entity.Id,
            entity.AgronomistId,
            entity.PlantationId,
            entity.AffiliatedAt,
            entity.DetachedAt
        );
    }

    public static List<AgronomistPlantationAffiliationResource> ToResourceListFromEntityList(
        List<AgronomistPlantationAffiliation> entities)
    {
        return entities.Select(ToResourceFromEntity).ToList();
    }
}