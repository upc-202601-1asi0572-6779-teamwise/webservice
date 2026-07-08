using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Entities;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;

namespace SmartPalmPlatform.API.SensorDataProcessing.Application.QueryServices;

public class AgronomicThresholdQueryService(
    IAgronomicThresholdRepository agronomicThresholdRepository
) : IAgronomicThresholdQueryService
{
    public async Task<List<AgronomicThreshold>> Handle(AgronomicThresholdQuery query)
    {
        var thresholds = await agronomicThresholdRepository.FindByIotDeviceMacAddress(
            query.IotDeviceMacAddress
        );

        if (thresholds.Count == 0)
            throw new KeyNotFoundException($"IoT device '{query.IotDeviceMacAddress}' not found.");

        return thresholds;
    }
}
