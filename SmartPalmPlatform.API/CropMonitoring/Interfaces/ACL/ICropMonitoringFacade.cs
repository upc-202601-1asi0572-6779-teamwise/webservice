namespace SmartPalmPlatform.API.CropMonitoring.Interfaces.ACL;

public interface ICropMonitoringFacade
{
    Task<bool> PlantationExistsAsync(int plantationId);
    Task<bool> SectorExistsAsync(int sectorId);
    Task<bool> SectorIsActiveAsync(int sectorId);
    Task<bool> PlantationHasActiveSectorsAsync(int plantationId);
}
