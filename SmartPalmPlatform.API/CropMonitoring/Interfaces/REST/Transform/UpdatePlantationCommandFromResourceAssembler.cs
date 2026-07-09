using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Commands;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.ValueObjects;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Transform;

public static class UpdatePlantationCommandFromResourceAssembler
{
    public static UpdatePlantationDetailsCommand ToCommandFromResource(
        int id,
        UpdatePlantationResource resource
    )
    {
        var location = new PlantationLocation(
            resource.address,
            resource.coordinates
        );

        return new UpdatePlantationDetailsCommand(
            id,
            resource.name,
            location,
            resource.hectares
        );
    }
}
