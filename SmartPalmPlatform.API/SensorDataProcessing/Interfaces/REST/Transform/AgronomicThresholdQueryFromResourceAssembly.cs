using SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;

namespace SmartPalmPlatform.API.SensorDataProcessing.Interfaces.REST.Transform;

public static class AgronomicThresholdQueryFromResourceAssembly
{
    public static AgronomicThresholdQuery ToQueryFromResource(string edgeMac, string iotMac)
    {
        return new AgronomicThresholdQuery(edgeMac, iotMac);
    }
}
