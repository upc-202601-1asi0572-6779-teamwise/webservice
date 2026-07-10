namespace SmartPalmPlatform.API.IAM.Interfaces.REST.Resources;

public record PlanResource(
    string type,
    string name,
    decimal price,
    string billingCycle,
    int? maxHectares,
    int? maxSensors,
    int? maxPlantationHistory
);
