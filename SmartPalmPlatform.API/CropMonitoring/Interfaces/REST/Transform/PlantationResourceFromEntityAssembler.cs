using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Aggregates;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Transform;

public static class PlantationResourceFromEntityAssembler
{
    public static PlantationResource ToResourceFromEntity(Plantation entity)
    {
        return new PlantationResource(
            entity.Id,
            entity.Name,
            entity.Location.Address,
            entity.Location.Coordinates,
            entity.Hectares,
            entity.Status.ToString(),
            entity.InstallationPlan.EstimatedSensors,
            entity.InstallationPlan.Message,
            entity.CreatedAt,
            entity.UpdatedAt
        );
    }

    public static PlantationDetailResource ToDetailResourceFromEntity(Plantation entity)
    {
        return new PlantationDetailResource(
            entity.Id,
            entity.Name,
            entity.Location.Address,
            entity.Location.Coordinates,
            entity.Hectares,
            entity.Status.ToString(),
            entity.InstallationPlan.EstimatedSensors,
            entity.InstallationPlan.Message,
            entity.CreatedAt,
            entity.UpdatedAt,
            entity.Sectors.Select(SectorResourceFromEntityAssembler.ToResourceFromEntity).ToList()
        );
    }
}
