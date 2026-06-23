using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Aggregates;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;

namespace SmartPalmPlatform.API.SensorDataProcessing.Application.QueryServices;

public class SensorReadingQueryService(ISensorReadingRepository sensorReadingRepository)
    : ISensorReadingQueryService
{
    public async Task<List<SensorReading>> Handle(SensorReadingQuery query)
    {
        return await sensorReadingRepository.FindByEdgeDeviceMacAddressAndMeasureTimeRange(
            query.EdgeDeviceMacAddress,
            query.From,
            query.To
        );
    }
}
