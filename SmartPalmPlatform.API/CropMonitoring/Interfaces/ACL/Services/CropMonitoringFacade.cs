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
        Console.WriteLine($"[INFO] [CropMonitoring] [CropMonitoringFacade] PlantationExistsAsync({plantationId})");
        var plantation = await plantationQueryService.Handle(
            new GetPlantationByIdQuery(plantationId)
        );
        Console.WriteLine($"[INFO] [CropMonitoring] [CropMonitoringFacade] PlantationExistsAsync({plantationId}) → {plantation is not null}");
        return plantation is not null;
    }

    public async Task<bool> SectorExistsAsync(int sectorId)
    {
        Console.WriteLine($"[INFO] [CropMonitoring] [CropMonitoringFacade] SectorExistsAsync({sectorId})");
        var sector = await sectorRepository.FindByIdAsync(sectorId);
        Console.WriteLine($"[INFO] [CropMonitoring] [CropMonitoringFacade] SectorExistsAsync({sectorId}) → {sector is not null}");
        return sector is not null;
    }

    public async Task<bool> SectorIsActiveAsync(int sectorId)
    {
        Console.WriteLine($"[INFO] [CropMonitoring] [CropMonitoringFacade] SectorIsActiveAsync({sectorId})");
        var sector = await sectorRepository.FindByIdAsync(sectorId);
        var active = sector is not null && sector.Status == SectorStatus.Active;
        Console.WriteLine($"[INFO] [CropMonitoring] [CropMonitoringFacade] SectorIsActiveAsync({sectorId}) → {active}");
        return active;
    }

    public async Task<bool> PlantationHasActiveSectorsAsync(int plantationId)
    {
        Console.WriteLine($"[INFO] [CropMonitoring] [CropMonitoringFacade] PlantationHasActiveSectorsAsync({plantationId})");
        var sectors = await sectorRepository.FindByPlantationIdAsync(plantationId);
        var hasActive = sectors.Any(s => s.Status == SectorStatus.Active);
        Console.WriteLine($"[INFO] [CropMonitoring] [CropMonitoringFacade] PlantationHasActiveSectorsAsync({plantationId}) → {hasActive} (total sectors: {sectors.Count()})");
        return hasActive;
    }
}
