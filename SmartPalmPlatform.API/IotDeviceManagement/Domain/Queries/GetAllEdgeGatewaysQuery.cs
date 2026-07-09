namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Queries;

// OwnerUserId null = sin restricción (vista de Administrator); con valor = solo los
// gateways de ese usuario.
public record GetAllEdgeGatewaysQuery(int? OwnerUserId);
