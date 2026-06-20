using SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;

namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.CommandServices;

public interface IDeviceStatusCommandService
{
    Task Handle(RegisterEdgeDeviceCommand command);
    Task Handle(RegisterIotDeviceCommand command);

    Task Handle(EdgeSynchronizationCommand command);
}
