using SmartPalmPlatform.API.CropMonitoring.Domain.Model.Enums;
using SmartPalmPlatform.API.CropMonitoring.Domain.Model.ValueObjects;

namespace SmartPalmPlatform.API.CropMonitoring.Domain.Model.Aggregates;

public class Plantation
{
    public int Id { get; private set; }
    public int PalmGrowerId { get; private set; }
    public string Name { get; private set; }
    public PlantationLocation Location { get; private set; }
    public decimal Hectares { get; private set; }
    public CropType CropType { get; private set; }
    public PlantationStatus Status { get; private set; }
    public InstallationPlan InstallationPlan { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public Plantation(
        int palmGrowerId,
        string name,
        PlantationLocation location,
        decimal hectares,
        CropType cropType,
        InstallationPlan installationPlan
    )
    {
        PalmGrowerId = palmGrowerId;
        Name = name;
        Location = location;
        Hectares = hectares;
        CropType = cropType;
        Status = PlantationStatus.Installing;
        InstallationPlan = installationPlan;
        CreatedAt = DateTime.UtcNow;
    }

    private Plantation() { }

    public void Activate()
    {
        if (Status is PlantationStatus.Active or PlantationStatus.Cancelled)
            throw new InvalidOperationException(
                $"Cannot activate plantation in '{Status}' status."
            );

        Status = PlantationStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, PlantationLocation location, decimal hectares)
    {
        if (Status == PlantationStatus.Cancelled)
            throw new InvalidOperationException("Cannot update a cancelled plantation.");

        Name = name;
        Location = location;
        Hectares = hectares;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == PlantationStatus.Cancelled)
            throw new InvalidOperationException("Plantation is already cancelled.");

        Status = PlantationStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}
