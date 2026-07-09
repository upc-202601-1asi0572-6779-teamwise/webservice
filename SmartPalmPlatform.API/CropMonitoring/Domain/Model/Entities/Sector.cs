using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Enums;

namespace SmartPalmPlatform.API.CropMonitoring.Domain.Model.Entities;

public class Sector
{
    public int Id { get; private set; }
    public int PlantationId { get; private set; }
    public string Name { get; private set; }
    public string IotDeviceMacAddress { get; private set; }
    public SectorStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ActivatedAt { get; private set; }

    public Sector(int plantationId, string name, string iotDeviceMacAddress)
    {
        PlantationId = plantationId;
        Name = name;
        IotDeviceMacAddress = iotDeviceMacAddress;
        Status = SectorStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    private Sector() { }

    public void Activate()
    {
        if (Status != SectorStatus.Pending)
            throw new InvalidOperationException($"Sector is already in '{Status}' status.");

        Status = SectorStatus.Active;
        ActivatedAt = DateTime.UtcNow;
    }

    public void SetMaintenance()
    {
        if (Status != SectorStatus.Active)
            throw new InvalidOperationException("Only active sectors can be set to maintenance.");

        Status = SectorStatus.Maintenance;
    }
}
