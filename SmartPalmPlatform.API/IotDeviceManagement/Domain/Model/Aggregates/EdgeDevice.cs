namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;

public class EdgeDevice
{
    public int Id { get; private set; }
    public string MacAddress { get; private set; }
    public int MonitoringZoneId { get; private set; }
    public int UserId { get; private set; }
    public DateTime LastConnectivityCheckAt { get; private set; } = DateTime.Now;
    public DateTime LastSyncAt { get; private set; } = DateTime.Now;
    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    public EdgeDevice(string macAddress, int monitoringZoneId, int userId)
    {
        this.MacAddress = macAddress;
        this.MonitoringZoneId = monitoringZoneId;
        this.UserId = userId;
    }

    public EdgeDevice()
        : this(string.Empty, 0, 0) { }

    public void Register()
    {
        this.LastConnectivityCheckAt = DateTime.Now;
    }

    // Un IotDevice registrado bajo este gateway debe pertenecer al mismo usuario.
    public void EnsureOwnedBy(int userId)
    {
        if (this.UserId != userId)
            throw new InvalidOperationException(
                "El dispositivo IoT no pertenece al mismo usuario que su Edge Gateway."
            );
    }

    public void SynchronizeEdgeData()
    {
        this.LastSyncAt = DateTime.Now;
        this.LastConnectivityCheckAt = DateTime.Now;
    }

    // if the device doesnt not receive any data for a certain period of time its considered offline
    public bool IsConnected => this.LastConnectivityCheckAt > DateTime.Now.AddMinutes(-1);
}
