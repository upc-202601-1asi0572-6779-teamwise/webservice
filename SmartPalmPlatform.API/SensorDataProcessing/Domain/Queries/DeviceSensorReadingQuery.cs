namespace SmartPalmPlatform.API.SensorDataProcessing.Domain.Queries;

// OwnerUserId null = sin restricción (Administrator); con valor, el dispositivo debe
// pertenecer a ese usuario o se trata como no encontrado.
public record DeviceSensorReadingQuery(
    string IotDeviceMacAddress,
    DateTime From,
    DateTime To,
    int Page,
    int Size,
    int? OwnerUserId
);
