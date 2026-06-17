namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.ValueObjects;

public class DeviceConfiguration
{
    public int SamplingIntervalMinutes { get; private set; } = 0;
    public string TransmissionMode { get; private set; } = "Push";
    public string RetryPolicy { get; private set; } = "RetryOnce";
    public int MaxOfflineStorageHours { get; private set; } = 0;

    public DeviceConfiguration(
        int samplingIntervalMinutes,
        string transmissionMode,
        string retryPolicy,
        int maxOfflineStorageHours
    )
    {
        this.SamplingIntervalMinutes = samplingIntervalMinutes;
        this.TransmissionMode = transmissionMode;
        this.RetryPolicy = retryPolicy;
        this.MaxOfflineStorageHours = maxOfflineStorageHours;
    }

    public DeviceConfiguration() { }

    public static DeviceConfiguration Default =>
        new DeviceConfiguration(60, "Push", "RetryOnce", 24);
}
