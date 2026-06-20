using SmartPalmPlatform.API.Shared.Domain.Model.Events;
using SmartPalmPlatform.API.Shared.Domain.Model.ValueObjects;

namespace SmartPalmPlatform.API.Shared.Domain.Events;

public record IotDeviceSynchronizationEvent(
    string EdgeDeviceMacAddress,
    List<SensorReadingPayload> SynchronizationReadings
) : IEvent;
