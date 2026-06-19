using SmartPalmPlatform.API.SensorDataProcessing.Domain.Commands;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.CommandServices;

public interface ISensorReadingCommandService
{
    Task Handle(ReadDeviceSensorsDataCommand command);
    Task Handle(UpdateAgronomicThresholdCommand command);
}
