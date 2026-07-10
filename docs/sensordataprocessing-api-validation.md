# SensorDataProcessing Bounded Context — API Reference

Este documento describe los endpoints del bounded context `SensorDataProcessing`. Cada sección muestra **qué espera recibir** (auth, headers, body, params), **qué responde** (status codes y shape de body), y **qué side effects** automáticos dispara la operación. No incluye historial de cambios; ese vive en los commits del repo.

---

## Convenciones generales

- **API base**: `/api/v1`
- **Auth header**: `Authorization: Bearer <jwt>` cuando se requiera (el JWT se obtiene de `/api/v1/authentication/sign-in` en el BC IAM; ver `docs/iam-api-validation.md`).
- **Suscripción activa**: algunos endpoints requieren suscripción activa (`[RequireActiveSubscription]`); el Administrator está exento.

---

## 1. Endpoints

### 1.1 `ReadDeviceSensorDataController`

Ruta base: `/api/v1/edge-gateways`

#### POST `/{gatewayMac}/sensor-readings` — Ingesta batch de readings

**Recibe**:

| Path | Tipo | Descripción |
|---|---|---|
| `gatewayMac` | string | MAC del edge gateway |

**Headers**: `Content-Type: application/json`

**Body** (`ReadDeviceSensorsDataResource`):

```json
{
  "devices": [
    {
      "deviceMac": "AA:BB:CC:DD:EE:01",
      "readings": [
        { "sensorType": "Temperature", "measuredAt": "2026-07-10T08:00:00Z", "value": 28.5 },
        { "sensorType": "Humidity",    "measuredAt": "2026-07-10T08:00:00Z", "value": 65.2 }
      ]
    }
  ],
  "syncedAt": "2026-07-10T08:00:00Z"
}
```

| Campo | Tipo | Notas |
|---|---|---|
| `devices[].deviceMac` | string | MAC del IoT device que emitió las readings |
| `devices[].readings[].sensorType` | string | Uno de: `Temperature`, `Humidity`, `PH`, `Luminosity`, `SoilMoisture` |
| `devices[].readings[].measuredAt` | ISO 8601 | Timestamp del reading |
| `devices[].readings[].value` | double | Valor medido |
| `syncedAt` | ISO 8601 | Cuándo el gateway emitió el batch |

**Responde**:

| Status | Significado |
|---|---|
| 201 | Ingesta aceptada (ver §2 para side effects) |
| 400 | Payload inválido (sensorType desconocido o JSON malformado) |

**Auth**: **anonymous** (`[AllowAnonymous]`). El endpoint puede ser llamado por el edge gateway sin token.

---

#### GET `/{gatewayMac}/sensor-readings` — Lecturas por gateway

**Recibe**:

| Path / Query | Tipo | Notas |
|---|---|---|
| `gatewayMac` | string | MAC del gateway |
| `from` | ISO 8601 (opcional) | Default `DateTime.MinValue` |
| `to` | ISO 8601 (opcional) | Default `DateTime.MaxValue` |
| `deviceMac` | string (opcional) | Filtra por un único IoT device |
| `page` | int (opcional) | Default 1 |
| `size` | int (opcional) | Default 100 |

**Auth**: bearer token + suscripción activa.

**Responde**: 200 con array de `SensorReadingViewResource`:

```json
{
  "edgeDeviceMacAddress": "AA:BB:CC:00:00:01",
  "iotDeviceMacAddress":  "AA:BB:CC:DD:EE:01",
  "sensorType":  "Temperature",
  "value": 28.5,
  "unit": "Celsius",
  "measuredAt": "2026-07-10T08:00:00Z"
}
```

---

### 1.2 `DeviceSensorReadingsController`

Ruta base: `/api/v1/devices`

#### GET `/{deviceMac}/sensor-readings` — Lecturas por device

**Recibe**:

| Path / Query | Tipo | Notas |
|---|---|---|
| `deviceMac` | string | MAC del IoT device |
| `from`, `to`, `page`, `size` | (opcional) | Mismos que el gateway |

**Auth**: bearer token + suscripción activa.

**Responde**:

| Status | Body |
|---|---|
| 200 | Array `SensorReadingViewResource` (puede ser vacío) |
| 404 | `{"message":"IoT device '..' not found."}` |

---

### 1.3 `AgronomicThresholdController`

Ruta base: `/api/v1/devices`

**Auth (ambos endpoints)**: bearer token + rol `Administrator` o `Agronomist` (`[Authorize(Roles = "Administrator,Agronomist")]`).

#### GET `/{deviceMac}/thresholds` — Thresholds por device

**Responde 200**: array de `AgronomicThresholdViewResource`:

```json
{
  "edgeMac": "AA:BB:CC:00:00:01",
  "iotMac":  "AA:BB:CC:DD:EE:01",
  "min":     15.0,
  "max":     35.0,
  "description": "Rango óptimo",
  "type":    "Temperature"
}
```

#### PATCH `/{deviceMac}/thresholds` — Update o crear threshold

**Body** (`UpdateAgronomicThresholdResource`):

```json
{ "type": "Temperature", "min": 15.0, "max": 35.0, "description": "Rango óptimo" }
```

| Campo | Tipo | Notas |
|---|---|---|
| `type` | string | Requerido. Uno de los 5 sensorTypes. |
| `min` | double (opcional) | Mínimo permitido |
| `max` | double (opcional) | Máximo permitido |
| `description` | string (opcional) | Texto libre |

Si el threshold no existe, se crea tomando el `edgeMac` inferido de otros thresholds del mismo device.

**Responde**:

| Status | Caso |
|---|---|
| 200 | Threshold actualizado o creado |
| 400 | ArgumentException (rango inválido) |
| 404 | Device no existe |

---

### 1.4 `SectorDataController`

Ruta base: `/api/v1/sectors`

**Auth (ambos endpoints)**: bearer token + suscripción activa.

#### GET `/{sectorId}/health` — Estado agregado del sector

**Recibe**: `sectorId` (int) en path.

**Responde 200**:

```json
{
  "sectorId": 1,
  "status":  2,
  "sensorDetails": [
    { "sensorType": "Humidity",    "value": 65.2, "minThreshold": 0,  "maxThreshold": 100, "isExceeded": false },
    { "sensorType": "Temperature", "value": 10,   "minThreshold": 15, "maxThreshold": 35,  "isExceeded": true  }
  ]
}
```

Escala de `status` inferida:

| Valor | Estado |
|---|---|
| 0 | Healthy (ningún sensor excedido) |
| 1 | Warning |
| 2 | Critical (al menos un sensor con `isExceeded: true`) |

**404** si el sector no existe o no tiene IoT device asignado.

#### GET `/{sectorId}/sensor-readings`

Mismo shape que el gateway pero filtrado por el IoT device del sector.

---

### 1.5 `AdminSensorDataController`

Ruta base: `/api/v1/admin/devices`

**Auth**: bearer token + rol `Administrator`.

| Verbo | Endpoint |
|---|---|
| GET  | `/{deviceMac}/thresholds` — equivalente admin al GET de `AgronomicThresholdController` |
| PATCH | `/{deviceMac}/thresholds` — equivalente admin al PATCH de `AgronomicThresholdController` |

Devuelven los mismos shapes que sus versiones sin prefijo `/admin/`.

---

## 2. Tipos de sensor y umbrales automáticos

Al registrar un IoT device (`POST /api/v1/edge-gateways/{gw}/iot-devices` en el BC IotDeviceManagement), el sistema crea **5 thresholds default**, uno por cada `SensorType`, con valores predeterminados:

| SensorType | min | max | Description inicial |
|---|---|---|---|
| `Humidity` | 0 | 100 | "Humidity Sensor Threshold" |
| `PH` | 0 | 100 | "PH Sensor Threshold" |
| `Luminosity` | 0 | 100 | "Luminosity Sensor Threshold" |
| `Temperature` | 10 | 40 | "Temperature Sensor Threshold" |
| `SoilMoisture` | 20 | 80 | "Soil Moisture Sensor Threshold" |

Estos valores se pueden sobreescribir después con `PATCH /devices/{mac}/thresholds`.

---

## 3. Side effects del flujo de ingesta

`POST /api/v1/edge-gateways/{gw}/sensor-readings` dispara automáticamente lo siguiente:

1. **Persistencia**: cada reading se guarda como `SensorReading` en la tabla correspondiente.
2. **Evaluación de threshold**: por cada reading, el sistema busca el `AgronomicThreshold` del `(IotDeviceMacAddress, SensorType)` y compara el valor.
3. **`ThresholdExceededEvent`**: si el valor cae fuera del rango, se publica el evento. El handler en el BC `AlertsAndNotifications` crea un `Alert` con `level: Critical` por defecto y dispara una notificación Firebase (silenciosamente omitida si no hay credenciales).
4. **`SensorReadingsIngestedEvent`**: al terminar la transacción, se publica este evento con el `syncedAt` del batch.

Como consecuencia, el sector asociado al IoT device puede pasar a `Critical` en `GET /sectors/{id}/health` si alguna reading excede.

---

## 4. Errores comunes

| Síntoma | Causa probable |
|---|---|
| 401 en POST readings pese a `[AllowAnonymous]` | Comportamiento esperado en versiones anteriores del código. Si ves este caso en la versión actual, hay una regresión en `RequireActiveSubscription`. |
| `GET /devices/{mac}/sensor-readings` 404 | La MAC no existe en la tabla de IoT devices. Primero registrar el device en IotDeviceManagement. |
| `GET /sectors/{id}/health` con `sensorDetails: []` | El sector no tiene readings todavía. Alimentar con un POST readings. |
| `PATCH /thresholds` 404 | El device no existe; verificar MAC en IotDeviceManagement. |
| `GET /thresholds` 403 con palmgrower | El rol PalmGrower no está en `[Authorize(Roles = "Administrator,Agronomist")]`. Solo Administrator y Agronomist pueden leer/mutar thresholds. |

---

## 5. Cómo reproducir con curl

### Ingestar readings (sin token)

```powershell
'{"devices":[{"deviceMac":"AA:BB:CC:DD:EE:01","readings":[{"sensorType":"Temperature","measuredAt":"2026-07-10T09:00:00Z","value":29.5}]}],"syncedAt":"2026-07-10T09:00:00Z"}' `
  | Out-File -FilePath C:\Temp\r.json -Encoding ascii -NoNewline

curl.exe -k -X POST `
  -H 'Content-Type: application/json' `
  --data-binary '@C:\Temp\r.json' `
  https://localhost:7277/api/v1/edge-gateways/AA:BB:CC:00:00:01/sensor-readings
# 201 con body vacío
```

Para evitar el bug del tooling PS (ver `docs/end-to-end-domain-validation.md` §7), usar siempre archivos: `WriteAllText` con `UTF8Encoding::new($false)` para UTF-8 sin BOM, u `Out-File -Encoding ascii` solo si el body no contiene caracteres no-ASCII.

### Consultar readings

```powershell
curl.exe -k -H "Authorization: Bearer $token" `
  'https://localhost:7277/api/v1/devices/AA:BB:CC:DD:EE:01/sensor-readings?page=1&size=10'
```

### Consultar health de un sector

```powershell
curl.exe -k -H "Authorization: Bearer $token" `
  'https://localhost:7277/api/v1/sectors/1/health'
```

### Ajustar threshold

```powershell
'{"type":"Temperature","min":15.0,"max":35.0,"description":"Rango optimo"}' `
  | Out-File -FilePath C:\Temp\t.json -Encoding ascii -NoNewline

curl.exe -k -X PATCH `
  -H "Authorization: Bearer $token" `
  -H 'Content-Type: application/json' `
  --data-binary '@C:\Temp\t.json' `
  https://localhost:7277/api/v1/devices/AA:BB:CC:DD:EE:01/thresholds
```

---

## 6. Referencias cruzadas

| Concepto | Archivo |
|---|---|
| Suscripción activa (gatekeeper) | `IAM/Infrastructure/Pipeline/Middleware/Attributes/RequireActiveSubscription.cs` |
| Auth bearer (JWT) | `IAM/Infrastructure/Tokens/JWT/Services/TokenService.cs` |
| Ingesta (command service) | `SensorDataProcessing/Application/CommandServices/SensorReadingCommandService.cs` |
| Repositorio de thresholds | `SensorDataProcessing/Infraestructure/Persistance/EFC/AgronomicThresholdRepository.cs` |
| Factory de readings por sensor | `SensorDataProcessing/Domain/Model/Factory/SensorReadingTypeFactory.cs` |
| Evento `ThresholdExceededEvent` | `Shared/Domain/Events/ThresholdExceededEvent.cs` |
| Subscriber de alerts | `AlertsAndNotifications/Application/Internal/EventHandlers/ThresholdExceededEventHandler.cs` |
| Notificación Firebase | `AlertsAndNotifications/Infrastructure/Firebase/Services/FirebaseNotificationService.cs` |
| Auto-seed de 5 thresholds (al registrar IoT device) | Side effect de `POST /edge-gateways/{gw}/iot-devices` en IotDeviceManagement |
| Listado de planes públicos (IAM) | `docs/iam-api-validation.md` |
| Flujo cross-context completo | `docs/end-to-end-domain-validation.md` |
