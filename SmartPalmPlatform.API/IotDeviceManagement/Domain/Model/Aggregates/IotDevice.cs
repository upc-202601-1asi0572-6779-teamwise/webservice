using SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.ValueObjects;

namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Aggregates;

public class IotDevice
{
    public int Id { get; private set; }
    public string SerialNumber { get; private set; }
    public int MonitoringZoneId { get; private set; }
    public DeviceActivationStatus ActivationStatus { get; private set; } =
        DeviceActivationStatus.Inactive;
    public DeviceConnectivityStatus ConnectivityStatus { get; private set; } =
        DeviceConnectivityStatus.Disconnected;
    public DeviceHealthStatus HealthStatus { get; private set; } = DeviceHealthStatus.Warning;
    public DeviceConfiguration Configuration { get; private set; } = DeviceConfiguration.Default;
    public DateTime LastSyncAt { get; private set; } = DateTime.Now;
    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    public IotDevice(string serialNumber, int monitoringZoneId)
    {
        this.SerialNumber = serialNumber;
        this.MonitoringZoneId = monitoringZoneId;
    }

    public IotDevice()
        : this(string.Empty, 0) { }

    public void Register()
    {
        this.ActivationStatus = DeviceActivationStatus.Active;
        this.ConnectivityStatus = DeviceConnectivityStatus.Connected;
        this.HealthStatus = DeviceHealthStatus.Healthy;
    }

    public void ConfigureSamplingParameters(DeviceConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    public void Activate()
    {
        this.ActivationStatus = DeviceActivationStatus.Active;
    }

    public void Deactivate()
    {
        this.ActivationStatus = DeviceActivationStatus.Inactive;
    }

    public void ActivateOfflineMode()
    {
        this.ActivationStatus = DeviceActivationStatus.Inactive;
        this.ConnectivityStatus = DeviceConnectivityStatus.OfflineMode;
        this.HealthStatus = DeviceHealthStatus.Critical;
    }

    public void RestoreConnectivity()
    {
        this.ActivationStatus = DeviceActivationStatus.Active;
        this.ConnectivityStatus = DeviceConnectivityStatus.Connected;
    }

    public void SynchronizeEdgeData()
    {
        this.LastSyncAt = DateTime.Now;
        this.ConnectivityStatus = DeviceConnectivityStatus.Connected;
    }

    public bool IsActive => this.ActivationStatus == DeviceActivationStatus.Active;
    public bool IsConnected => this.ConnectivityStatus == DeviceConnectivityStatus.Connected;
}
