using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Aggregates;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;

public interface ISensorReadingRepository : IBaseRepository<SensorReading>
{
    public Task<List<SensorReading>> FindByEdgeDeviceMacAddressAndMeasureTimeRange(
        string edgeDeviceMacAddress,
        DateTime from,
        DateTime to,
        string? iotDeviceMacAddress,
        int page,
        int size
    );

    public Task<List<SensorReading>> FindByIotDeviceMacAddressAndMeasureTimeRange(
        string iotDeviceMacAddress,
        DateTime from,
        DateTime to,
        int page,
        int size
    );
}
