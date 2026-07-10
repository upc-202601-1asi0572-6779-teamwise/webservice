using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.DomainServices;
using SmartPalmPlatform.API.Shared.Domain.Model.Enums;

namespace SmartPalmPlatform.API.SensorDataProcessing.Application.DomainServices;

public class SensorTypeDomainService : ISensorTypeDomainService
{
    public bool TryParseSensorType(string value, out SensorType result)
    {
        return Enum.TryParse(value, ignoreCase: true, out result);
    }

    public bool IsValidSensorType(string value)
    {
        return Enum.TryParse<SensorType>(value, ignoreCase: true, out _);
    }
}