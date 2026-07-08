using SmartPalmPlatform.API.Shared.Domain.Model.Events;

namespace SmartPalmPlatform.API.Shared.Domain.Events;

public record EdgeDeviceRegisteredEvent(string EdgeDeviceMacAddress) : IEvent;
