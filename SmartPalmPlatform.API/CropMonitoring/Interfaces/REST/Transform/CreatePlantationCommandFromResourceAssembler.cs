using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Commands;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Enums;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.ValueObjects;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Resources;

namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.REST.Transform;

public static class CreatePlantationCommandFromResourceAssembler
{
    public static CreatePlantationCommand ToCommandFromResource(
        int palmGrowerId,
        CreatePlantationResource resource
    )
    {
        var location = new PlantationLocation(
            resource.address,
            resource.coordinates
        );

        var cropType = Enum.TryParse<CropType>(resource.cropType, true, out var parsed)
            ? parsed
            : CropType.Other;

        return new CreatePlantationCommand(
            palmGrowerId,
            resource.name,
            location,
            resource.hectares,
            cropType
        );
    }
}
