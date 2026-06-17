using SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;

namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Services;

public interface IDeviceStatusCommandService
{
    Task Handle(RegisterDeviceCommand command);
    Task Handle(ActivateDeviceCommand command);
    Task Handle(DeactivateDeviceCommand command);
}
