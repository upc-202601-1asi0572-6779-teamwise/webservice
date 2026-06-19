using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Aggregates;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Entities;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.DomainServices;

namespace SmartPalmPlatform.API.SensorDataProcessing.Application.DomainServices;

public class ThresholdEvaluationService : IThresholdEvaluationService
{
    public bool IsThresholdExceeded(SensorReading reading, AgronomicThreshold threshold)
    {
        return threshold.IsExceededBy(reading.Value);
    }
}
