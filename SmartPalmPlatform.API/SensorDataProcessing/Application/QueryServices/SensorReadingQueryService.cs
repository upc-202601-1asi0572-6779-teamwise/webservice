using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Aggregates;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;

namespace SmartPalmPlatform.API.SensorDataProcessing.Application.QueryServices;

public class SensorReadingQueryService(
    ISensorReadingRepository sensorReadingRepository,
    IAgronomicThresholdRepository agronomicThresholdRepository
) : ISensorReadingQueryService
{
    public async Task<List<SensorReading>> Handle(SensorReadingQuery query)
    {
        return await sensorReadingRepository.FindByEdgeDeviceMacAddressAndMeasureTimeRange(
            query.EdgeDeviceMacAddress,
            query.From,
            query.To,
            query.IotDeviceMacAddress,
            query.Page,
            query.Size
        );
    }

    public async Task<List<SensorReading>> Handle(DeviceSensorReadingQuery query)
    {
        var exists = await agronomicThresholdRepository.ExistsByIotDeviceMacAddress(
            query.IotDeviceMacAddress
        );
        if (!exists)
            throw new KeyNotFoundException(
                $"IoT device '{query.IotDeviceMacAddress}' not found."
            );

        return await sensorReadingRepository.FindByIotDeviceMacAddressAndMeasureTimeRange(
            query.IotDeviceMacAddress,
            query.From,
            query.To,
            query.Page,
            query.Size
        );
    }
}
