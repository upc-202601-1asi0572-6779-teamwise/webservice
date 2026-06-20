using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Aggregates;
using SmartPalmPlatform.API.Shared.Domain.Repositories;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;

public interface ISensorReadingRepository : IBaseRepository<SensorReading>
{
    public Task<List<SensorReading>> FindByEdgeDeviceMacAddressAndMeasureTimeRange(
        string edgeDeviceMacAddress,
        DateTime from,
        DateTime to
    );
}
