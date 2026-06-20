using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Entities;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;

public interface IAgronomicThresholdQueryService
{
    public Task<List<AgronomicThreshold>> Handle(AgronomicThresholdQuery query);
}
