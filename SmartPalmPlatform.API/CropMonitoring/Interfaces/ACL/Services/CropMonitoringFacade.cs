using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Enums;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Queries;
using SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services.QueryServices;

namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.ACL.Services;

public class CropMonitoringFacade(
    IPlantationQueryService plantationQueryService,
    ISectorRepository sectorRepository
) : ICropMonitoringFacade
{
    public async Task<bool> PlantationExistsAsync(int plantationId)
    {
        var plantation = await plantationQueryService.Handle(
            new GetPlantationByIdQuery(plantationId)
        );
        return plantation is not null;
    }

    public async Task<bool> SectorExistsAsync(int sectorId)
    {
        var sector = await sectorRepository.FindByIdAsync(sectorId);
        return sector is not null;
    }

    public async Task<bool> SectorIsActiveAsync(int sectorId)
    {
        var sector = await sectorRepository.FindByIdAsync(sectorId);
        return sector is not null && sector.Status == SectorStatus.Active;
    }

    public async Task<bool> PlantationHasActiveSectorsAsync(int plantationId)
    {
        var sectors = await sectorRepository.FindByPlantationIdAsync(plantationId);
        return sectors.Any(s => s.Status == SectorStatus.Active);
    }
}
