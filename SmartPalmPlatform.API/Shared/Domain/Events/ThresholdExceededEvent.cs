using SmartPalmPlatform.API.Shared.Domain.Model.Enums;
using SmartPalmPlatform.API.Shared.Domain.Model.Events;

namespace SmartPalmPlatform.API.Shared.Domain.Events;

public record ThresholdExceededEvent(
    string EdgeDeviceMacAddress,
    SensorType SensorType,
    double ReadingValue,
    double ThresholdMin,
    double ThresholdMax
) : IEvent;
