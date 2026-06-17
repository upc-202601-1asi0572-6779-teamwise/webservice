using SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;

namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Services;

public interface IEdgeSynchronizationService
{
    Task Handle(EdgeSynchronizationCommand command);
}
