using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Aggregates;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Transform;

public static class PlantationResourceFromEntityAssembler
{
    public static PlantationResource ToResourceFromEntity(Plantation entity)
    {
        return new PlantationResource(
            entity.Name,
            entity.Location.Address,
            entity.Location.Coordinates,
            entity.Hectares,
            entity.CropType.ToString(),
            entity.Status.ToString(),
            entity.InstallationPlan.EstimatedSensors,
            entity.InstallationPlan.Message,
            entity.CreatedAt,
            entity.UpdatedAt
        );
    }

    public static PlantationDetailResource ToDetailResourceFromEntity(Plantation entity)
    {
        // Since Sector isn't a navigation property on Plantation aggregate,
        // this would normally be populated separately
        return new PlantationDetailResource(
            entity.Name,
            entity.Location.Address,
            entity.Location.Coordinates,
            entity.Hectares,
            entity.CropType.ToString(),
            entity.Status.ToString(),
            entity.InstallationPlan.EstimatedSensors,
            entity.InstallationPlan.Message,
            entity.CreatedAt,
            entity.UpdatedAt,
            new List<SectorResource>()
        );
    }
}
