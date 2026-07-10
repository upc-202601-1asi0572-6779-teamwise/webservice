namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;

public record SectorSensorDataQuery(
    int SectorId,
    DateTime From,
    DateTime To
);