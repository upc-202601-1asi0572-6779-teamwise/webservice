using SmartPalmPlatform.API.CropMonitoring.Domain.Model.ValueObjects;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services.DomainServices;

namespace SmartPalmPlatform.API.CropMonitoring.Application.Internal.DomainServices;

public class InstallationPlanService : IInstallationPlanService
{
    public InstallationPlan CalculatePlan(decimal hectares)
    {
        var sensorCount = Math.Max(1, (int)Math.Ceiling(hectares / 10));

        var message = sensorCount == 1
            ? $"This plantation requires 1 sensor sector for monitoring."
            : $"This plantation requires {sensorCount} sensor sectors ({hectares} ha / 10 ha per sensor).";

        return new InstallationPlan(sensorCount, message);
    }
}
