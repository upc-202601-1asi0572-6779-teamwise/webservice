namespace SmartPalmPlatform.API.CropMonitoring.Domain.Model.Entities;

public class AgronomistPlantationAffiliation
{
    public int Id { get; private set; }
    public int AgronomistId { get; private set; }
    public int PlantationId { get; private set; }
    public DateTime AffiliatedAt { get; private set; }
    public DateTime? DetachedAt { get; private set; }

    public AgronomistPlantationAffiliation(int agronomistId, int plantationId)
    {
        AgronomistId = agronomistId;
        PlantationId = plantationId;
        AffiliatedAt = DateTime.UtcNow;
    }

    private AgronomistPlantationAffiliation() { }

    public void Detach()
    {
        if (DetachedAt.HasValue)
            throw new InvalidOperationException("Affiliation is already detached.");

        DetachedAt = DateTime.UtcNow;
    }
}