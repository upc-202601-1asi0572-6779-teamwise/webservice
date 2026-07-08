using SmartPalmPlatform.API.Shared.Domain.Model.Events;

namespace SmartPalmPlatform.API.Shared.Domain.Events;

public record SensorReadingsIngestedEvent(string EdgeDeviceMacAddress, DateTime SyncedAt) : IEvent;
