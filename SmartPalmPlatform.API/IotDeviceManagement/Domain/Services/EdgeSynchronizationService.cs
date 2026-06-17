using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;

namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Services;

public class EdgeSynchronizationService
{
    public bool ValidateSynchronization(IotDevice device)
    {
        return device.IsActive;
    }

    public List<SensorReading> MapReadingsToChronologicalOrder(List<SensorReading> readings)
    {
        return readings
            .Select(reading => reading.Clone())
            .OrderBy(reading => reading.Timestamp)
            .ToList();
    }

    public void SynchronizeStoredData(IotDevice device)
    {
        device.SynchronizeEdgeData();
    }
}
