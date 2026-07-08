using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Aggregates;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;

public interface IPlantationRepository : IBaseRepository<Plantation>
{
    Task<List<Plantation>> FindByPalmGrowerIdAsync(int palmGrowerId);
    Task<Plantation?> FindByIdWithSectorsAsync(int id);
}
