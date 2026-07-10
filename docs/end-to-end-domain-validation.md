# End-to-end Domain Validation Report

Fecha: 2026-07-10
Branch: `develop`
Ambiente: local (PostgreSQL 17 en `localhost:5432`, DB `smartpalm_dev`)
API base dev: `https://localhost:7277/api/v1`

Este documento describe el recorrido analítico que hice sobre tres bounded contexts del backend (`IAM`, `CropMonitoring`, `IotDeviceManagement`) usando `curl` desde PowerShell contra el server HTTPS directo. Está orientado a un dev que necesita entender **cómo se conectan los flujos del dominio** y **qué trampas conocidas hay hoy en el código**.

No pretende ser exhaustivo: cada endpoint fue probado con la profundidad necesaria para entender el flujo. Los detalles completos por endpoint están en `docs/iam-api-validation.md`. Acá el foco es **end-to-end**.

---

## 1. Alcance y resultado de la validación

| Bounded context | Endpoints probados | Cobertura | Estado |
|---|---|---|---|
| `IAM` | 7 (sign-in + 3 admin/users + 2 users + 1 subscriptions/plans) | parcial | ✅ |
| `CropMonitoring` | 10 (5 user-side plantations + 3 admin + 2 sectors) | parcial | ✅ |
| `IotDeviceManagement` | 4 (1 edge gateway + 2 IoT device register + 1 list devices) | parcial | ✅ |

**Resultado global:** se completó un caso feliz completo desde "DB vacía" hasta "plantation `Active` con todos los sectores `Active` y devices reales registrados en un edge gateway". En el camino quedaron identificados 4 bugs/observaciones menores que se listan en §5.

---

## 2. Estado final de la DB tras la validación

### Users

| id | username | rol | contraseña (hasheada con BCrypt) |
|---|---|---|---|
| 1 | admin | Administrator | (sembrado al startup) |
| 2 | palmgrower01 | PalmGrower | `Grower#2026` |
| 3 | agronomist01 | Agronomist | `Agro#2026` |

El admin semilla siempre con `admin/admin` (ver `Program.cs:330-353`).

### Suscripciones

| userId | plan | status | precio | periodicity |
|---|---|---|---|---|
| 2 | Seed | Active | 149 USD/mes | Mensual |
| 3 | Seed | Active | 149 USD/mes | Mensual |
| 1 | — | — | — | admin exento de RequireActiveSubscription |

### Pagos

| id | userId | amount | status | periodStart | periodEnd |
|---|---|---|---|---|---|
| 1 | 2 | 149 | Completed | 2026-07-10T07:37:39 | 2026-08-10T07:37:39 |
| 2 | 3 | 149 | Completed | 2026-07-10T07:37:40 | 2026-08-10T07:37:40 |

Cada pago activa automáticamente la suscripción (no hay endpoint "activate" separado; ver §4.1).

### Plantations

| id | name | hectares | ownerId | status | estimatedSensors | sectores asignados |
|---|---|---|---|---|---|---|
| 1 | Hacienda San Pedro | 48.00 | 1 (admin) | Active | 5 | 5 (todos Active) |
| 2 | Parcela PalmGrower | 12.00 | 2 (palmgrower01) | Active | 2 | 2 (todos Active) |

### Sectors

Todos los sectores quedaron en `Active` con `activatedAt` real:

| id | plantationId | name | iotMac | status | activatedAt |
|---|---|---|---|---|---|
| 1 | 1 | Sector Norte | AA:BB:CC:DD:EE:01 | Active | 2026-07-10T07:51:28 |
| 2 | 1 | Sector Sur | AA:BB:CC:DD:EE:02 | Active | 2026-07-10T07:51:28 |
| 3 | 1 | Sector Este | AA:BB:CC:DD:EE:03 | Active | 2026-07-10T07:51:28 |
| 4 | 1 | Sector Oeste | AA:BB:CC:DD:EE:04 | Active | 2026-07-10T07:51:29 |
| 5 | 1 | Sector Central | AA:BB:CC:DD:EE:05 | Active | 2026-07-10T07:51:29 |
| 6 | 2 | Sector Norte | BB:CC:DD:EE:FF:01 | Active | 2026-07-10T07:51:29 |
| 7 | 2 | Sector Sur | BB:CC:DD:EE:FF:02 | Active | 2026-07-10T07:51:29 |

### Edge Gateways / IoT Devices

1 edge gateway registrado: `AA:BB:CC:00:00:01` en `monitoringZoneId=1`. Los 7 IoT devices listados arriba están asociados al gateway.

---

## 3. Flujo end-to-end validado

El happy path completo es:

```
[admin] POST /plantation              → Plantation id=1, status=Installing, est=5
[admin] POST /plantation/{1}/sectors  → 5 sectors, todos Pending
[admin] POST /edge-gateways           → EdgeGateway con MAC y monitoringZoneId
[admin] POST /edge-gateways/{gw}/iot-devices × 5 (con {iotMac, plantationId})
                                       → por cada POST se dispara IotDeviceRegisteredEvent
                                       → handler busca el sector con esa MAC y lo activa
                                       → cuando sectorCount >= est, plantation.Activate()
```

Verificación final: `GET /plantations/1` devuelve `status: "Active"` con `updatedAt` actualizado.

Para arrancar de cero (lo que replicaría un dev), los pasos exactos están en §6.

---

## 4. Hallazgos del dominio

### 4.1 Activación de plantation es event-driven

**Único** path hacia `PlantationStatus.Active` es el handler `CropMonitoring/Application/Internal/EventHandlers/CropMonitoringIotDeviceRegisteredEventHandler.cs:38-71`:

1. El evento `IotDeviceRegisteredEvent` se publica cuando se registra un IoT device bajo un edge gateway.
2. El handler busca la plantation por `plantationId` (la del `IotDeviceRegistrationResource`).
3. Busca el sector pre-creado con `IotDeviceMacAddress == event.IotDeviceMacAddress`.
4. Si existe y está `Pending`, llama `existing.Activate()` y persiste (`sectorRepository.Update` + `uow.CompleteAsync`).
5. Cuenta sectores totales de la plantation con `sectorRepository.FindByPlantationIdAsync`.
6. Si `count >= InstallationPlan.EstimatedSensors` y `plantation.Status == Installing` → llama `plantation.Activate()` y persiste.

**Consecuencia operativa**: Para activar una plantation hay que **pre-asignar sectores con la MAC que tendrá el dispositivo** y luego registrar el dispositivo. Si registrás un IoT device con una MAC que NO está pre-asignada a ningún sector, el handler igual crea un sector nuevo (líneas 53-62), lo activa y lo cuenta. Esa rama no se ejerció en este round.

**Consecuencia para IDs**: el handler ignora silenciosamente plantations con status `Cancelled` o que no existen (`return;`), pero no lo notifica al cliente. Un dev que llame a `POST /edge-gateways/{gw}/iot-devices` con `plantationId` inválido no recibe error; tiene que verificar el resultado con un GET separado.

### 4.2 Las suscripciones se activan automáticamente al primer pago

`RequireActiveSubscription.cs:11-31` consulta `IIamContextFacade.HasActiveSubscriptionAsync(user.Id)`. Lo que considera "activa" no es solo que exista la suscripción sino que tenga pagos asociados.

El flujo validado:
1. `POST /admin/subscriptions {userId, planType}` → crea la sub con `status: Pending`.
2. El admin (cualquiera) llama `POST /admin/subscriptions/users/{userId}/payments {amount}` → crea la transacción `PaymentTransaction` con `status: Completed` y persiste.
3. Subsiguientes `GET /subscriptions` → devuelve `SubscriptionResource` completo con `status: Active`.

No hay endpoint para "activar" o "cambiar status" de una suscripción sin pasar por el endpoint de pagos.

### 4.3 Authorization ortogonal entre Roles y Subscripción

El backend aplica dos filtros independientes:

| Filtro | Atributo | Aplica a nivel de | Administrador | PalmGrower | Agronomist |
|---|---|---|---|---|---|
| Roles | `[Authorize(Roles = "...")]` | Controller / Action | siempre pasa | si el rol está en la lista | si el rol está en la lista |
| Subscripción | `[RequireActiveSubscription]` | Action / Controller | exento (siempre pasa) | requiere activa | requiere activa |

`[RequireActiveSubscription.cs:21]` exime al Administrator. Para los demás roles, requiere que la suscripción esté activa (= con pago procesado). Si no la tienen, devuelve **403 Forbid** (sin body, vía `ForbidResult()`).

**Implicación directa:** el `Agronomist` no puede usar `/plantations` aunque tenga una subscripción activa. El atributo `[Authorize(Roles = "Administrator,PalmGrower")]` del `PlantationsController` lo bloquea antes de llegar a `RequireActiveSubscription`. Esto es coherente con la separación de dominio: el agrónomo no es dueño de plantaciones.

---

## 5. Bugs y observaciones encontrados

### 5.1 `installationMessage` nunca se recalcula (confirmado en estado Active)

**Severidad**: baja-media — solo informativo, no afecta reglas del dominio, pero miente.

**Síntoma**: tras PATCH o asignación completa de sectores, el campo `installationMessage` del `PlantationResource` sigue diciendo `"This plantation requires N sensor sectors (X ha / 10 ha per sensor)"` con los hectares y el conteo **del momento de creación**. Lo confirmé dos veces:

- Antes de Active: id=1 con 48 ha, mensaje decía `(45.50 ha / 10 ha per sensor)`.
- Después de Active: id=1 sigue con `(45.50 ha / 10 ha per sensor)` pese a que la plantation ya está `Active`.

**Causa**: `PlantedAt` se construye pasando `installationPlan` en el constructor (`Plantation.cs:34`). `InstallationPlanService.CalculatePlan(decimal hectares)` (`InstallationPlanService.cs:6-16`) solo se invoca en el `CreatePlantationCommand` handler. `UpdateDetails(...)` (`Plantation.cs:51-60`) NO recalcula el plan, y el handler de sectors tampoco lo toca.

**Fix propuesto**: invocar `installationPlanService.CalculatePlan(Hectares)` dentro de `UpdateDetails(...)` y reasignar `InstallationPlan`. Alternativa: derivar el mensaje en runtime en el assembler (`PlantationResourceFromEntityAssembler`) en vez de persistirlo.

### 5.2 `GET /subscriptions` devuelve dos shapes distintos según status

**Severidad**: baja — rotura de contrato del DTO.

`UserSubscriptionController.cs:42-45` ramifica por `subscription.Status`:

- Si `status == Active` → devuelve `SubscriptionResource` (8 campos con `planType`, `planName`, `price`, `status`, `startDate`, `endDate`, `billingCycle`, `createdAt`).
- Otros status → devuelve un anónimo `{status, planName, amountDue}` (3 campos).

**Impacto**: un cliente que arme un DTO strict para `SubscriptionResource` va a recibir respuestas con campos faltantes en el caso "Pending" o "Cancelled".

**Fix propuesto**: extraer la respuesta a un único resource completo, o bien devolver siempre `SubscriptionResource` y aceptar `startDate`/`endDate` nulls.

### 5.3 `POST /admin/plantations/{id}/sectors` no valida el IoT device

**Severidad**: baja — por diseño, pero conviene documentar.

El endpoint acepta cualquier MAC que venga en `AssignSectorResource.iotDeviceMacAddress` sin verificar que exista en la tabla de IoT devices. Crea un sector `Pending` que queda esperando el eventual `IotDeviceRegisteredEvent` para activarse.

**Consecuencia**: si nunca se registra el IoT device con esa MAC, el sector queda `Pending` indefinidamente y el conteo de sectores `Active` no avanza. La plantation seguirá en `Installing` para siempre, salvo que algún IoT device con esa MAC se registre en el futuro.

**Fix propuesto**: agregar validación opcional o marcar el sector con un `ExpectsIoTDevice=true` separado del estado Pendiente.

### 5.4 `SyncActiveSubscription` y el campo `subscriptionId` del User

**Severidad**: informativa.

`User.cs:22` declara `public int? SubscriptionId { get; private set; }` con un método `UpdateSubscription(int? subscriptionId)` que no parece estar siendo invocado desde los endpoints probados. La relación entre `Subscription.Id` y `User.SubscriptionId` puede haberse modelado pero no estar cableada. No verifiqué a fondo.

---

## 6. Cómo reproducir (devs nuevos)

Asume Postgres 17 corriendo, API levantada localmente (`dotnet run` desde `SmartPalmPlatform.API` o desde VS vía `F5`/`http` profile), y un usuario admin sembrado al primer arranque (`admin/admin`).

### Paso 1 — login de admin y obtener token

```powershell
'{"username":"admin","password":"admin"}' | Out-File -FilePath C:\Temp\login.json -Encoding ascii -NoNewline

curl.exe -sS -k -X POST `
  -H 'Content-Type: application/json' `
  --data-binary '@C:\Temp\login.json' `
  https://localhost:7277/api/v1/authentication/sign-in
```

Esto devuelve `{userId:1, username:"admin", token:"..."}`. Guardá el token en una variable.

### Paso 2 — crear uno o más usuarios de prueba (PalmGrower/Agronomist) con admin

```powershell
$body = '{"username":"palmgrower01","password":"Grower#2026","email":"pg1@smartpalm.com","fullName":"Palm Grower One","role":"PalmGrower"}'
$body | Out-File -FilePath C:\Temp\u.json -Encoding ascii -NoNewline

curl.exe -sS -k -X POST `
  -H 'Content-Type: application/json' `
  -H "Authorization: Bearer $adminToken" `
  --data-binary '@C:\Temp\u.json' `
  https://localhost:7277/api/v1/admin/users
```

### Paso 3 — activar suscripción del PalmGrower

```powershell
$body = '{"userId":2,"planType":"Seed"}'
$body | Out-File -FilePath C:\Temp\sub.json -Encoding ascii -NoNewline
curl.exe -sS -k -X POST -H 'Content-Type: application/json' -H "Authorization: Bearer $adminToken" --data-binary '@C:\Temp\sub.json' https://localhost:7277/api/v1/admin/subscriptions

'{"amount":149}' | Out-File -FilePath C:\Temp\pay.json -Encoding ascii -NoNewline
curl.exe -sS -k -X POST -H 'Content-Type: application/json' -H "Authorization: Bearer $adminToken" --data-binary '@C:\Temp\pay.json' https://localhost:7277/api/v1/admin/subscriptions/users/2/payments
```

Sin este paso, el PalmGrower no puede usar `/plantations` — recibe 403.

### Paso 4 — crear plantation y asignar sectores

```powershell
'{"name":"Mi Plantacion","hectares":12,"address":"Direccion","coordinates":""}' | Out-File -FilePath C:\Temp\p.json -Encoding ascii -NoNewline
curl.exe -sS -k -X POST -H 'Content-Type: application/json' -H "Authorization: Bearer $adminToken" --data-binary '@C:\Temp\p.json' https://localhost:7277/api/v1/plantations
# Devuelve 201 con id y estimatedSensors.

# Para cada sector (estimatedSensors veces):
'{"iotDeviceMacAddress":"AA:BB:CC:DD:EE:01","sectorName":"Sector Norte"}' | Out-File -FilePath C:\Temp\s.json -Encoding ascii -NoNewline
curl.exe -sS -k -X POST -H 'Content-Type: application/json' -H "Authorization: Bearer $adminToken" --data-binary '@C:\Temp\s.json' https://localhost:7277/api/v1/admin/plantations/{plantationId}/sectors
```

La plantation queda en `Installing`, sectores en `Pending`.

### Paso 5 — registrar el edge gateway y los IoT devices

```powershell
# Edge gateway
'{"edgeMac":"AA:BB:CC:00:00:01","monitoringZoneId":1}' | Out-File -FilePath C:\Temp\edge.json -Encoding ascii -NoNewline
curl.exe -sS -k -X POST -H 'Content-Type: application/json' -H "Authorization: Bearer $adminToken" --data-binary '@C:\Temp\edge.json' https://localhost:7277/api/v1/edge-gateways

# IoT devices — uno por sector con la plantationId correcta
'{"iotMac":"AA:BB:CC:DD:EE:01","plantationId":1}' | Out-File -FilePath C:\Temp\d.json -Encoding ascii -NoNewline
curl.exe -sS -k -X POST -H 'Content-Type: application/json' -H "Authorization: Bearer $adminToken" --data-binary '@C:\Temp\d.json' https://localhost:7277/api/v1/edge-gateways/AA:BB:CC:00:00:01/iot-devices
```

Tras el último IoT device registrado, `GET /plantations/{id}` devuelve `status: "Active"`.

---

## 7. Notas metodológicas

### 7.1 PowerShell 5.1 + curl.exe

Probado y funcionando: usar **siempre** `--data-binary @archivo` con JSON escrito a disco antes. `curl.exe -d 'json_inline'` rompe el body en PS 5.1 con bytes corruptos que el backend rechaza con 400.

### 7.2 HTTPS self-signed

Sumar `-k` a curl.exe por el certificado self-signed del Kestrel de dev. O usar directamente `https://localhost:7277/api/v1` para evitar el 307 de `UseHttpsRedirection`.

### 7.3 Sin scripts reutilizables

No se generó script reproducible en este round. Lo razonable para integrar a CI sería:
- Un `tests/integration/iam.http` con las requests a `IAM` (formato `.http` compatible con VS Code REST Client).
- Un `tests/integration/cropmonitoring.http` con la secuencia de plantaciones.
- Un `tests/integration/iotdevicemanagement.http` con la secuencia edge gateway + IoT devices.

Idealmente parametrizados con variables para tokens y plantationId.

### 7.4 Limpieza de DB

No hay endpoint REST para borrar plantaciones, sectores, IoT devices, edge gateways, suscripciones, ni pagos. Para resetear el ambiente local hay que usar SQL directo contra `smartpalm_dev` o borrar la DB con `DROP DATABASE smartpalm_dev` y dejar que `EnsureCreated()` la regenere en el próximo arranque (con la salvedad de que ese path reset TODO, incluyendo el seed del admin).

---

## 8. Referencias de código fuente

| Concepto | Archivo (path relativo a `SmartPalmPlatform.API/`) |
|---|---|
| Seed del admin | `Program.cs:330-353` |
| RequireActiveSubscription (y exención de admin) | `IAM/Infrastructure/Pipeline/Middleware/Attributes/RequireActiveSubscription.cs` |
| Plans disponibles (Seed/Harvest/Custom) | `IAM/Domain/Model/ValueObjects/SubscriptionPlan.cs` |
| UserRole enum (3 roles) | `IAM/Domain/Model/Enums/UserRole.cs` |
| Plantation aggregate + Activate | `CropMonitoring/Domain/Model/Aggregates/Plantation.cs:40-49` |
| Sector aggregate + Activate | `CropMonitoring/Domain/Model/Entities/Sector.cs:26-33` |
| Cascada IoT → sector → plantation (handler central) | `CropMonitoring/Application/Internal/EventHandlers/CropMonitoringIotDeviceRegisteredEventHandler.cs` |
| InstallationPlan calc (solo se llama en CREATE) | `CropMonitoring/Application/Internal/DomainServices/InstallationPlanService.cs` |
| Plantation controller (User-side, roles Admin+PalmGrower + RequireActiveSubscription) | `CropMonitoring/Interfaces/REST/PlantationsController.cs` |
| Plantation controller (Admin) | `CropMonitoring/Interfaces/REST/Admin/AdminPlantationsController.cs` |
| Sectors (Admin) | `CropMonitoring/Interfaces/REST/PlantationSectorsController.cs` |
| Edge gateway register | `IotDeviceManagement/Interfaces/REST/DeviceAuthenticationController.cs` |
| Auth token (sin `ClaimTypes.Role` en el JWT — usar `OnTokenValidated` para enriquecer) | `IAM/Infrastructure/Tokens/JWT/Services/TokenService.cs:38-49` |

---

## 9. Para los próximos validadores

- Los BCs restantes sin tocar: `SensorDataProcessing`, `AlertsAndNotifications`, `AgronomicRecommendation`, `FieldTechnicalManagement`. Vale un round similar para cada uno.
- El bug del `installationMessage` (§5.1) está listo para un fix de 5 minutos si alguien quiere tomarlo.
- Falta probar `DELETE /admin/plantations/{id}/sectors/{sectorId}` y la cascada opuesta (eliminar sector → recalcular activation). Útil para entender el modelo.
- El endpoint `DELETE /subscriptions` (cancelación propia) no fue ejercitado — queda como caso interesante.
