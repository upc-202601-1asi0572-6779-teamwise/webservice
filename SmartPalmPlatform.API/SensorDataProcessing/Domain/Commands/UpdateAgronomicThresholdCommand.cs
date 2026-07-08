using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Commands;

public record UpdateAgronomicThresholdCommand(
    string IotDeviceMacAddress,
    SensorType Type,
    double? Min,
    double? Max,
    string? Description
);
