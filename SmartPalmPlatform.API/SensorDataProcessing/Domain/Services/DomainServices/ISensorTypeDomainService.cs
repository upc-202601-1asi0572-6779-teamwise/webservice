using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.DomainServices;

public interface ISensorTypeDomainService
{
    bool TryParseSensorType(string value, out SensorType result);
    bool IsValidSensorType(string value);
}