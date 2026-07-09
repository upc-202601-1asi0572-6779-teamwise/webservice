using SmartPalmPlatform.API.CropMonitoring.Domain.Model.ValueObjects;

namespace SmartPalmPlatform.API.CropMonitoring.Domain.Services.DomainServices;

public interface IInstallationPlanService
{
    InstallationPlan CalculatePlan(decimal hectares);
}
