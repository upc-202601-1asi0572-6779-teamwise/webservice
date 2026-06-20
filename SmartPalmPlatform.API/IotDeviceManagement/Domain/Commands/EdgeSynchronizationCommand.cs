using SmartPalmPlatform.API.Shared.Domain.Model.ValueObjects;

namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Commands;

public record EdgeSynchronizationCommand(string EdgeDeviceMac, List<SensorReadingPayload> readings);
