using SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;

public interface ISectorHealthQueryService
{
    Task<SectorHealthResult?> Handle(GetSectorHealthQuery query);
}