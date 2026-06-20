using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;

namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.DomainServices;

public interface IEdgeSynchronizationService
{
    List<SensorReading> MapReadingsToChronologicalOrder(List<SensorReading> readings);
}
