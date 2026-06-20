namespace SmartPalmPlatform.API.IotDeviceManagement.Domain.Model.Entities;

public class EdgeRegistry
{
    public int Id { get; private set; }
    public string EdgeMacAddress { get; private set; }
    public string IotDeviceMacAddresses { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    public EdgeRegistry(string edgeMacAddress, string iotDeviceMacAddresses)
    {
        this.EdgeMacAddress = edgeMacAddress;
        this.IotDeviceMacAddresses = iotDeviceMacAddresses;
    }

    public EdgeRegistry()
        : this(string.Empty, string.Empty) { }
}
