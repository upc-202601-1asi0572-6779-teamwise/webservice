# IAM Bounded Context — Referencia de API

Fecha: 2026-07-10
Branch: `develop`
Base de datos local: `smartpalm_dev` (PostgreSQL 17)
API base en dev: `https://localhost:7277/api/v1`

Esta guía documenta los endpoints del bounded context IAM validados contra el backend. Cada endpoint describe explícitamente **qué espera recibir** (auth, headers, body, params) y **qué responde** (status codes y ejemplos de body). Para cualquier endpoint listado como **no validado**, los detalles completos están en el código fuente bajo `SmartPalmPlatform.API/IAM/Interfaces/REST/`.

---

## Convenciones generales

### Autenticación
Todas las requests — excepto donde se indique lo contrario — requieren el header:

```
Authorization: Bearer <jwt>
```

El JWT se obtiene de `POST /authentication/sign-in`. Tiene una validez de **7 días** desde la emisión (`TokenService.cs:45`). Una vez expirado, el usuario debe volver a autenticarse.

### Formato de los bodies
Todos los bodies son JSON con `Content-Type: application/json`. Los nombres de campos en requests son **camelCase** (e.g. `fullName`). Los nombres de campos en responses también son **camelCase**.

### Respuestas de error
Las respuestas `4xx` siguen el formato ProblemDetails de ASP.NET Core:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.x",
  "title": "...",
  "status": 401,
  "traceId": "00-..."
}
```

Los `400` por validación de input además incluyen `errors` con detalle por campo.

---

## 1. Autenticación

### POST /authentication/sign-in

**Controller:** `AuthenticationController` (`AuthenticationController.cs`)
**Auth:** No requiere (endpoint público).

#### Request

**Headers:**
```
Content-Type: application/json
```

**Body** (`SignInResource`):

```json
{
  "username": "admin",
  "password": "admin"
}
```

| Campo | Tipo | Requerido | Validación |
|---|---|---|---|
| `username` | string | sí | no vacío |
| `password` | string | sí | no vacío |

#### Responses

**200 OK** — login exitoso. Body `AuthenticatedUserResource`:

```json
{
  "userId": 1,
  "username": "admin",
  "token": "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3ODQyNzE3MzcsImlhdCI6MTc4MzY2NjkzN30..."
}
```

> Nota: el JWT tiene exactamente 388 caracteres en `admin/admin`. Si la longitud es muy distinta, el body puede haberse corrompido (ver "Notas metodológicas").

**401 Unauthorized** — credenciales inválidas:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.2",
  "title": "Unauthorized",
  "status": 401,
  "traceId": "..."
}
```

#### Lógica interna

`UserCommandService.Handle(SignInCommand)` (`UserCommandService.cs:19-34`):

1. Busca el usuario por `username` (`UserRepository.FindByUsernameAsync`).
2. Verifica la password contra el hash BCrypt (`HashingService.VerifyPassword`).
3. Si ambas pasan, genera JWT y devuelve `(user, token)`.
4. Si falla cualquiera, lanza `Exception("Invalid username or password")` que el controller traduce a **401**.

#### Credenciales sembradas al primer arranque

`Program.cs:330-353` siembra el primer `Administrator` con `admin/admin`. Si las credenciales son incorrectas, revise logs del startup: debe aparecer `[INFO] [Seed] Admin user created (admin/admin).` la primera vez, o `Admin user already exists, skipping.` en arranques posteriores.

---

## 2. Gestión de usuarios (Admin)

### POST /admin/users

**Controller:** `AdminUsersController` (`Admin/AdminUsersController.cs`)
**Auth:** `[Authorize(Roles = "Administrator")]` — token JWT del admin.

#### Request

**Headers:**
```
Authorization: Bearer <jwt-admin>
Content-Type: application/json
```

**Body** (`CreateUserResource`):

```json
{
  "username": "palmgrower01",
  "password": "Grower#2026",
  "email": "pg1@smartpalm.com",
  "fullName": "Palm Grower One",
  "role": "PalmGrower"
}
```

| Campo | Tipo | Requerido | Validación (`UserDomainService.cs`) |
|---|---|---|---|
| `username` | string | sí | 3-50 chars, único |
| `password` | string | sí | mínimo 6 chars |
| `email` | string | sí | formato `x@y.z` |
| `fullName` | string | sí | no validado más allá de `required` |
| `role` | string | sí | uno de: `Administrator`, `Agronomist`, `PalmGrower` (case-insensitive) |

La lógica en `UserCommandService.Handle(CreateUserCommand)` (`UserCommandService.cs:36-56`):

1. Valida formato de cada campo.
2. Parsea `role` con `Enum.TryParse<UserRole>` (case-insensitive). Si falla, devuelve `"Invalid role '<valor>'. Valid roles: Administrator, Agronomist, PalmGrower"`.
3. Verifica unicidad de `username`. Si existe, devuelve `"Username '<x>' is already taken."`.
4. Hashea la password con BCrypt.
5. Persiste el `User`.

#### Responses

**201 Created** — usuario creado. Body `UserResource`:

```json
{
  "id": 2,
  "username": "palmgrower01",
  "email": "pg1@smartpalm.com",
  "fullName": "Palm Grower One",
  "role": "PalmGrower",
  "status": "Active"
}
```

Header `Location` apunta a la ruta del nuevo recurso (no es estrictamente usado).

**400 Bad Request** — validación de input falló o username duplicado:

```json
{
  "message": "Username 'palmgrower01' is already taken."
}
```

**401 Unauthorized** — sin token o token inválido.

#### Notas operativas

- El atributo `[RequireActiveSubscription]` (`RequireActiveSubscription.cs:21`) exime a usuarios con rol `Administrator`, por lo que el admin puede llamar este endpoint sin tener una suscripción activa.
- El sistema **no expone** un endpoint público de sign-up. Todo usuario debe ser creado por un admin.

---

### GET /admin/users

**Auth:** `[Authorize(Roles = "Administrator")]`.

#### Request

Sin body. Solo header `Authorization: Bearer <jwt-admin>`.

#### Responses

**200 OK** — listado completo:

```json
[
  {
    "id": 1,
    "username": "admin",
    "email": "admin@smartpalm.com",
    "fullName": "System Administrator",
    "role": "Administrator",
    "status": "Active"
  },
  {
    "id": 2,
    "username": "palmgrower01",
    "email": "pg1@smartpalm.com",
    "fullName": "Palm Grower One",
    "role": "PalmGrower",
    "status": "Active"
  }
]
```

**401 Unauthorized** — sin token o token inválido.

---

### GET /admin/users/{userId}

**Auth:** `[Authorize(Roles = "Administrator")]`.

#### Path params

| Nombre | Tipo | Descripción |
|---|---|---|
| `userId` | int | id numérico del usuario. Si es inválido (no entero), ASP.NET devuelve 400 automáticamente. |

#### Responses

**200 OK** — `UserResource` (mismo shape que en POST).

**404 Not Found** — no existe usuario con ese id:

```json
{
  "message": "User not found."
}
```

**401 Unauthorized** — sin token o token inválido.

---

## 3. Endpoints "Users" (alias de Admin)

El controller `UsersController` (`UsersController.cs`) está montado en `/api/v1/users` y **también** exige `[Authorize(Roles = "Administrator")]`. Sus endpoints son funcionalmente idénticos a los de `/admin/users`:

| Endpoint | Equivalente en `/admin/users` |
|---|---|
| `GET /api/v1/users/{id}` | `GET /api/v1/admin/users/{userId}` |
| `GET /api/v1/users` | `GET /api/v1/admin/users` |

Un usuario no-admin que llame a `/users/{id}` con su token será rechazado con 401 (no tiene el rol).

---

## 4. Inventario de endpoints IAM adicionales (no validados en esta ronda)

Estos endpoints existen en el codebase y se enumeran aquí como referencia. **No fueron probados** durante esta validación y los detalles de body/responses deben confirmarse en el código fuente antes de consumirlos.

### UserSubscriptionController (`api/v1/subscriptions`, requiere auth — cualquier rol)

| Método | Ruta | Función |
|---|---|---|
| GET | `/subscriptions` | Devuelve la suscripción del usuario autenticado |
| DELETE | `/subscriptions` | Cancela la suscripción del usuario autenticado |
| GET | `/subscriptions/payments` | Lista los pagos del usuario autenticado |

Las tres usan `HttpContext.Items["User"]` para extraer el `userId` (no reciben ese dato en el request).

### SubscriptionPlansController (`api/v1/subscriptions/plans`, endpoint público)

| Método | Ruta | Auth | Función |
|---|---|---|---|
| GET | `/subscriptions/plans` | ninguno | Lista los planes de suscripción disponibles (Seed, Pro, Enterprise, etc., definidos en `SubscriptionPlan.cs`). |

### AdminSubscriptionController (`api/v1/admin/subscriptions`, requiere `Administrator`)

| Método | Ruta | Función |
|---|---|---|
| POST | `/admin/subscriptions` | Crea una suscripción para un usuario específico |
| GET | `/admin/subscriptions` | Lista todas las suscripciones |
| GET | `/admin/subscriptions/{subscriptionId}` | Obtiene una suscripción por id |
| POST | `/admin/subscriptions/users/{userId}/payments` | Procesa un pago para la suscripción de un usuario |

---

## 5. Roles del sistema

El sistema define **3 roles** (no 2):

| Valor numérico | Nombre | Quién puede asignarlo | Notas |
|---|---|---|---|
| 0 | `Administrator` | sólo otro Administrator | Único con acceso a `/api/v1/admin/*` y `/api/v1/users*`. Excluido del chequeo de suscripción activa. |
| 1 | `Agronomist` | sólo un Administrator | Usuario profesional que gestiona plantaciones, recomendaciones y reportes. |
| 2 | `PalmGrower` | sólo un Administrator | Dueño/explotador de la plantación; recibe lecturas de sensores y notificaciones. |

### Definición

`IAM/Domain/Model/Enums/UserRole.cs`:

```csharp
public enum UserRole
{
    Administrator = 0,
    Agronomist    = 1,
    PalmGrower    = 2,
}
```

### Validación al asignar

`UserDomainService.ParseUserRole(string role)` (`UserDomainService.cs:55-60`) usa `Enum.TryParse<UserRole>` con `ignoreCase: true`. Cualquier valor fuera de los tres enumera un `ArgumentException` con el mensaje literal:

```
Invalid role '<valor>'. Valid roles: Administrator, Agronomist, PalmGrower
```

Esto hace imposible persistir un rol fuera del enum: la fuente de verdad es la enumeración en `UserRole.cs`. No hay JSON, tabla ni mapping paralelo.

### Quién puede asignar roles

Únicamente un `Administrator` puede crear usuarios (y, por lo tanto, asignar roles) vía `POST /api/v1/admin/users`. No hay endpoint para que un usuario cambie su propio rol. El flujo real es:

1. Al primer arranque, el seed en `Program.cs:330-353` crea el único admin (`admin/admin`).
2. Ese admin (o cualquier admin posterior) es el único vector para crear usuarios adicionales con cualquier rol.

---

## 6. Estado de `smartpalm_dev` tras las pruebas de validación

| id | username | rol | email | fullName |
|---|---|---|---|---|
| 1 | admin | Administrator | admin@smartpalm.com | System Administrator |
| 2 | palmgrower01 | PalmGrower | pg1@smartpalm.com | Palm Grower One |
| 3 | agronomist01 | Agronomist | agro1@smartpalm.com | Agronomist One |

Los tres roles del enum están representados al menos una vez. Las passwords en claro son `admin`, `Grower#2026` y `Agro#2026` respectivamente — cambia antes de cualquier deploy.

---

## 7. Notas metodológicas

### Cómo autenticarse paso a paso (ejemplo mínimo, Windows)

```powershell
# 1. Login
'{"username":"admin","password":"admin"}' | Out-File -FilePath C:\Temp\login.json -Encoding ascii -NoNewline

curl.exe -sS -k -X POST `
  -H 'Content-Type: application/json' `
  --data-binary '@C:\Temp\login.json' `
  https://localhost:7277/api/v1/authentication/sign-in

# 2. Capturar token (parsear JSON con ConvertFrom-Json en PowerShell)
# 3. Llamar a un endpoint protegido
curl.exe -sS -k -X GET `
  -H "Authorization: Bearer <token>" `
  https://localhost:7277/api/v1/admin/users
```

### Gotchas de tooling en Windows + PowerShell 5.1

| Herramienta | Problema | Solución |
|---|---|---|
| `curl.exe -d '{json}'` inline | PowerShell 5.1 corrompe el cuerpo inline (se reportan `Content-Length` menores al largo real del JSON). El backend rechaza con 400. | Usar siempre `--data-binary @archivo` con JSON escrito a disco. |
| `Invoke-WebRequest -SkipHttpErrorCheck` | No existe en PS 5.1. | Usar `curl.exe`. |
| `Invoke-RestMethod --Body` | PS 5.1 no acepta el doble guión. | Usar `curl.exe`. |
| HTTPS self-signed | `curl` rechaza sin `-k`. | Usar `-k` durante dev. |
| HTTP 5055 → HTTPS 7277 | `UseHttpsRedirection` redirige. Pegar a 5055 te agrega un 307 al inicio. | Trabajar directo contra 7277 evita el redirect y reduce ruido en logs. |

### Validación final recomendada

Antes de cerrar la validación por hoy, vale correr este flujo smoke:

1. `POST /authentication/sign-in` con `admin/admin` → debe devolver 200.
2. `GET /admin/users` con ese token → debe devolver 200 con 3 elementos (admin, palmgrower01, agronomist01).
3. `GET /admin/users/2` → debe devolver el usuario con id=2 (`palmgrower01`).
4. `GET /admin/users` **sin** token → debe devolver 401.
