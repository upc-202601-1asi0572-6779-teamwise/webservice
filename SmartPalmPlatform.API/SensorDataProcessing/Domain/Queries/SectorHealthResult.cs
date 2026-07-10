using SmartPalmPlatform.API.SensorDataProcessing.Domain.Model.Enums;

namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;

public record SectorHealthResult(
    int SectorId,
    SectorHealthStatus Status,
    List<SensorHealthDetail> SensorDetails
);

public record SensorHealthDetail(
    string SensorType,
    double Value,
    double? MinThreshold,
    double? MaxThreshold,
    bool IsExceeded
);