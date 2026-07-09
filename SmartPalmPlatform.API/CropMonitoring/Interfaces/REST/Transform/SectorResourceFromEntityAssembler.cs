using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Entities;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Transform;

public static class SectorResourceFromEntityAssembler
{
    public static SectorResource ToResourceFromEntity(Sector entity)
    {
        return new SectorResource(
            entity.Id,
            entity.Name,
            entity.IotDeviceMacAddress,
            entity.Status.ToString(),
            entity.CreatedAt,
            entity.ActivatedAt
        );
    }
}
