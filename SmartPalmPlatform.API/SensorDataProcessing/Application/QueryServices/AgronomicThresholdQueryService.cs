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
        var thresholds =
            await agronomicThresholdRepository.FindByEdgeDeviceMacAddressAndIotDeviceMacAddress(
                query.EdgeDeviceMacAddress,
                query.IotDeviceMacAddresses
            );

        return thresholds;
    }
}
