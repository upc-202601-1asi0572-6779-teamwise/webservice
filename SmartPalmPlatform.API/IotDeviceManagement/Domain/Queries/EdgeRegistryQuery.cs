namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Queries;

// OwnerUserId null = sin restricción (Administrator); con valor, el gateway debe
// pertenecer a ese usuario o se trata como no encontrado.
public record EdgeRegistryQuery(string EdgeDeviceMac, int? OwnerUserId);
