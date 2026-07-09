using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Entities;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;

public interface ISectorRepository : IBaseRepository<Sector>
{
    Task<List<Sector>> FindByPlantationIdAsync(int plantationId);
    Task<Sector?> FindByIotDeviceMacAddressAsync(string iotDeviceMacAddress);
}
