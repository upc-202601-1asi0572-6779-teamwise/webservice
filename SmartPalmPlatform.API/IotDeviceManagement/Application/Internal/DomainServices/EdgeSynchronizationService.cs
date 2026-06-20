using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.DomainServices;

namespace SmartPalmPlatform.API.IotDeviceManagement.Application.Internal.DomainServices;

public class EdgeSynchronizationService : IEdgeSynchronizationService
{
    public List<SensorReading> MapReadingsToChronologicalOrder(List<SensorReading> readings)
    {
        return readings.OrderBy(reading => reading.Timestamp).ToList();
    }
}
