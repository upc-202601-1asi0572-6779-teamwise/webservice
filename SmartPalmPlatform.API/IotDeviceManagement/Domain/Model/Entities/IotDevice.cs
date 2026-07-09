namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;

public class IotDevice
{
    public int Id { get; private set; }
    public string MacAddress { get; private set; }
    public string EdgeDeviceMacAddress { get; private set; }
    public int UserId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    public IotDevice(string edgeDeviceMacAddress, string iotDeviceMacAddress, int userId)
    {
        this.EdgeDeviceMacAddress = edgeDeviceMacAddress;
        this.MacAddress = iotDeviceMacAddress;
        this.UserId = userId;
    }

    public IotDevice()
        : this(string.Empty, string.Empty, 0) { }
}
