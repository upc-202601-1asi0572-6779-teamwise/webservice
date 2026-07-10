using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Aggregates;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Queries;

namespace SmartPalmPlatform.API.CropMonitoring.Domain.Services.QueryServices;

public interface IPlantationQueryService
{
    Task<Plantation?> Handle(GetPlantationByIdQuery query);
    Task<List<Plantation>> Handle(GetPlantationsByUserIdQuery query);
    Task<IEnumerable<Plantation>> Handle(GetAllPlantationsQuery query);
}
