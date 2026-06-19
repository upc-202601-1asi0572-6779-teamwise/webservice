using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Aggregates;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Entities;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.DomainServices;

public interface IThresholdEvaluationService
{
    public bool IsThresholdExceeded(SensorReading reading, AgronomicThreshold threshold);
}
