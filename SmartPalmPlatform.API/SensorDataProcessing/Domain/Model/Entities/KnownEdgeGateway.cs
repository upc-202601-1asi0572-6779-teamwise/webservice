namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Entities;

// Réplica local (vía IotDeviceManagement.EdgeDeviceRegisteredEvent) de qué
// gateways están registrados, para validar existencia sin depender del
// repositorio de IotDeviceManagement.
public class KnownEdgeGateway
{
    public int Id { get; set; } = 0;
    public string EdgeDeviceMacAddress { get; set; } = string.Empty;

    public KnownEdgeGateway() { }

    public KnownEdgeGateway(string edgeDeviceMacAddress)
    {
        this.EdgeDeviceMacAddress = edgeDeviceMacAddress;
    }
}
