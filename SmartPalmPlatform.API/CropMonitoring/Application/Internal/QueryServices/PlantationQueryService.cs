using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Aggregates;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Queries;
using SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services.QueryServices;

namespace SmartPalmPlatform.API.CropMonitoring.Application.Internal.QueryServices;

public class PlantationQueryService(
    IPlantationRepository plantationRepository
) : IPlantationQueryService
{
    public async Task<Plantation?> Handle(GetPlantationByIdQuery query)
    {
        return await plantationRepository.FindByIdWithSectorsAsync(query.Id);
    }

    public async Task<List<Plantation>> Handle(GetPlantationsByUserIdQuery query)
    {
        return await plantationRepository.FindByPalmGrowerIdAsync(query.UserId);
    }

    public async Task<IEnumerable<Plantation>> Handle(GetAllPlantationsQuery query)
    {
        return await plantationRepository.ListAsync();
    }
}
