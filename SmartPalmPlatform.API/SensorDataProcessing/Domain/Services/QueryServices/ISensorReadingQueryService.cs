using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Aggregates;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;

public interface ISensorReadingQueryService
{
    Task<List<SensorReading>> Handle(SensorReadingQuery query);
    Task<List<SensorReading>> Handle(DeviceSensorReadingQuery query);
    Task<List<SensorReading>> Handle(SectorSensorDataQuery query);
}
