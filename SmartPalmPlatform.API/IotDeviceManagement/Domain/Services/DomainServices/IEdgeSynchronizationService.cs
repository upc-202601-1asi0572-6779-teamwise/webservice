using SmartPalmPlatform.API.Shared.Domain.Model.ValueObjects;

namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.DomainServices;

public interface IEdgeSynchronizationService
{
    List<SensorReadingPayload> MapReadingsToChronologicalOrder(List<SensorReadingPayload> readings);
}
