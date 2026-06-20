using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.DomainServices;
using SmartPalmPlatform.API.Shared.Domain.Model.ValueObjects;

namespace SmartPalmPlatform.API.IotDeviceManagement.Application.Internal.DomainServices;

public class EdgeSynchronizationService : IEdgeSynchronizationService
{
    public List<SensorReadingPayload> MapReadingsToChronologicalOrder(
        List<SensorReadingPayload> readings
    )
    {
        return readings.OrderBy(reading => reading.MeasuredAt).ToList();
    }
}
