
### 4.2.5. Bounded Context: Crop Monitoring Dashboard

Este bounded context se encarga de consolidar y presentar la información de monitoreo del cultivo a los usuarios finales. Provee vistas del estado general de las plantaciones, alertas activas, historial de variables, comparación entre zonas, generación de reportes técnicos y visualización de recomendaciones publicadas. Interactúa con los bounded contexts de Sensor Data Processing (BC-02), Alert & Notification (BC-03) y Agronomic Recommendation (BC-04) como consumidor de datos procesados.

#### 4.2.5.1. Domain Layer

Contiene la lógica de negocio pura y las entidades principales relacionadas con la visualización del estado del cultivo, la consolidación de indicadores agronómicos, la generación de reportes técnicos y el acceso a historial de variables monitoreadas.

---

**Aggregate 1: CropHealthSnapshot**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| CropHealthSnapshot | Entity (Aggregate Root) | Representa una captura consolidada del estado de salud de una zona de monitoreo en un momento dado. Agrega múltiples parámetros sensoriales y aplica lógica de priorización para determinar el estado general (Óptimo, En riesgo, Crítico). Es la raíz del agregado que resuelve el pain point de conflicto entre parámetros. |

**Attributes**

| Nombre | Tipo de dato | Visibilidad | Descripción |
|--------|-------------|-------------|-------------|
| id | UUID | Private | Identificador único del snapshot. |
| zoneId | UUID | Private | Identificador de la zona de monitoreo evaluada. |
| plantationId | UUID | Private | Identificador de la plantación a la que pertenece la zona. |
| overallStatus | CropStatus (enum) | Private | Estado consolidado del cultivo: OPTIMAL, AT_RISK, CRITICAL. |
| parameterSummaries | List\<ParameterSummary\> | Private | Resumen de cada variable monitoreada con su valor actual y estado individual. |
| dominantRiskFactor | String | Private | Parámetro que determina el estado general cuando hay conflicto entre indicadores. |
| evaluatedAt | LocalDateTime | Private | Fecha y hora en que se generó el snapshot. |
| activeAlertCount | Integer | Private | Cantidad de alertas activas en la zona al momento de la evaluación. |

**Methods**

| Nombre | Tipo de retorno | Visibilidad | Descripción |
|--------|----------------|-------------|-------------|
| evaluate(parameters: List\<ParameterSummary\>) | Void | Public | Recalcula el estado general aplicando la regla de priorización: el peor estado individual prevalece (worst-wins). |
| isZoneCritical() | Boolean | Public | Indica si la zona se encuentra en estado crítico. |
| getDominantRisk() | String | Public | Retorna el parámetro que domina la evaluación de riesgo. |

---

**Value Object: ParameterSummary**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| ParameterSummary | Value Object | Resumen del estado de una variable monitoreada individual dentro de un snapshot. Es inmutable. |

**Attributes**

| Nombre | Tipo de dato | Visibilidad | Descripción |
|--------|-------------|-------------|-------------|
| parameterType | ParameterType (enum) | Private | Tipo de variable: SOIL_MOISTURE, TEMPERATURE, PH, CONDUCTIVITY. |
| currentValue | Double | Private | Último valor registrado del parámetro. |
| unit | String | Private | Unidad de medida (°C, %, mS/cm, pH). |
| status | ParameterStatus (enum) | Private | Estado individual: NORMAL, WARNING, CRITICAL. |
| thresholdMin | Double | Private | Umbral mínimo configurado para este parámetro. |
| thresholdMax | Double | Private | Umbral máximo configurado para este parámetro. |

---

**Enum: CropStatus**

| Código | Descripción |
|--------|-------------|
| OPTIMAL | Todas las variables dentro de rangos normales. |
| AT_RISK | Al menos una variable en estado WARNING, ninguna en CRITICAL. |
| CRITICAL | Al menos una variable en estado CRITICAL. |

**Enum: ParameterType**

| Código | Descripción |
|--------|-------------|
| SOIL_MOISTURE | Humedad del suelo. |
| TEMPERATURE | Temperatura ambiental. |
| PH | pH del suelo. |
| ELECTRICAL_CONDUCTIVITY | Conductividad eléctrica del suelo. |

**Enum: ParameterStatus**

| Código | Descripción |
|--------|-------------|
| NORMAL | Valor dentro del rango aceptable. |
| WARNING | Valor cercano a los límites del umbral. |
| CRITICAL | Valor fuera del rango aceptable. |

---

**Aggregate 2: PlantationOverview**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| PlantationOverview | Entity (Aggregate Root) | Representa la vista consolidada del estado de una plantación completa, agregando los snapshots de todas sus zonas de monitoreo. Permite al dueño del cultivo y al agrónomo obtener una visión rápida de la salud general de la plantación. |

**Attributes**

| Nombre | Tipo de dato | Visibilidad | Descripción |
|--------|-------------|-------------|-------------|
| id | UUID | Private | Identificador único de la vista de plantación. |
| plantationId | UUID | Private | Identificador de la plantación. |
| plantationName | String | Private | Nombre de la plantación. |
| totalZones | Integer | Private | Número total de zonas de monitoreo. |
| criticalZones | Integer | Private | Cantidad de zonas en estado CRITICAL. |
| atRiskZones | Integer | Private | Cantidad de zonas en estado AT_RISK. |
| optimalZones | Integer | Private | Cantidad de zonas en estado OPTIMAL. |
| overallStatus | CropStatus (enum) | Private | Estado consolidado de la plantación (worst-wins entre zonas). |
| lastUpdatedAt | LocalDateTime | Private | Fecha y hora de la última actualización de datos. |
| zoneSnapshots | List\<CropHealthSnapshot\> | Private | Lista de snapshots de cada zona. |

**Methods**

| Nombre | Tipo de retorno | Visibilidad | Descripción |
|--------|----------------|-------------|-------------|
| consolidateStatus() | Void | Public | Recalcula el estado general de la plantación a partir de los snapshots de sus zonas. |
| getZonesByPriority() | List\<CropHealthSnapshot\> | Public | Retorna las zonas ordenadas por criticidad descendente para priorización. |
| hasDataGap() | Boolean | Public | Indica si alguna zona no tiene datos recientes (más de 24h sin actualización). |

---

**Aggregate 3: SensorTimeSeries**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| SensorTimeSeries | Entity (Aggregate Root) | Representa el historial de una variable monitoreada en una zona específica durante un período de tiempo. Permite al agrónomo analizar tendencias y evolución de las condiciones del cultivo. |

**Attributes**

| Nombre | Tipo de dato | Visibilidad | Descripción |
|--------|-------------|-------------|-------------|
| id | UUID | Private | Identificador único de la serie temporal. |
| zoneId | UUID | Private | Identificador de la zona de monitoreo. |
| parameterType | ParameterType (enum) | Private | Variable monitoreada. |
| startDate | LocalDateTime | Private | Inicio del rango temporal consultado. |
| endDate | LocalDateTime | Private | Fin del rango temporal consultado. |
| dataPoints | List\<TimeSeriesDataPoint\> | Private | Puntos de datos de la serie temporal. |
| trendDirection | TrendDirection (enum) | Private | Dirección de la tendencia: RISING, STABLE, FALLING. |

**Methods**

| Nombre | Tipo de retorno | Visibilidad | Descripción |
|--------|----------------|-------------|-------------|
| calculateTrend() | TrendDirection | Public | Calcula la dirección de la tendencia a partir de los data points. |
| getAverageValue() | Double | Public | Retorna el promedio de los valores en el período. |
| getMaxValue() | Double | Public | Retorna el valor máximo registrado en el período. |
| getMinValue() | Double | Public | Retorna el valor mínimo registrado en el período. |
| hasAnomalies() | Boolean | Public | Detecta si existen valores fuera de rango en la serie. |

---

**Value Object: TimeSeriesDataPoint**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| TimeSeriesDataPoint | Value Object | Punto individual de datos en una serie temporal. Inmutable. |

**Attributes**

| Nombre | Tipo de dato | Visibilidad | Descripción |
|--------|-------------|-------------|-------------|
| timestamp | LocalDateTime | Private | Momento de la lectura. |
| value | Double | Private | Valor registrado. |
| status | ParameterStatus (enum) | Private | Estado del valor respecto a los umbrales. |

---

**Enum: TrendDirection**

| Código | Descripción |
|--------|-------------|
| RISING | La variable muestra tendencia ascendente. |
| STABLE | La variable se mantiene estable. |
| FALLING | La variable muestra tendencia descendente. |

---

**Aggregate 4: TechnicalReport**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| TechnicalReport | Entity (Aggregate Root) | Representa un reporte técnico generado por el agrónomo que resume el estado de una plantación, incluyendo datos de monitoreo, alertas, observaciones y recomendaciones. Soporta un ciclo de vida de borrador a publicado. |

**Attributes**

| Nombre | Tipo de dato | Visibilidad | Descripción |
|--------|-------------|-------------|-------------|
| id | UUID | Private | Identificador único del reporte. |
| plantationId | UUID | Private | Identificador de la plantación evaluada. |
| authorId | UUID | Private | Identificador del agrónomo autor del reporte. |
| title | String | Private | Título del reporte. |
| summary | String | Private | Resumen ejecutivo del estado del cultivo. |
| sections | List\<ReportSection\> | Private | Secciones del reporte con contenido detallado. |
| status | ReportStatus (enum) | Private | Estado del reporte: DRAFT, PUBLISHED. |
| generatedAt | LocalDateTime | Private | Fecha de generación del borrador. |
| publishedAt | LocalDateTime | Private | Fecha de publicación (null si aún es borrador). |
| snapshotReferences | List\<UUID\> | Private | Referencias a los CropHealthSnapshots incluidos en el reporte. |

**Methods**

| Nombre | Tipo de retorno | Visibilidad | Descripción |
|--------|----------------|-------------|-------------|
| publish() | Void | Public | Cambia el estado del reporte de DRAFT a PUBLISHED. Lanza excepción si el reporte ya fue publicado. |
| addSection(section: ReportSection) | Void | Public | Agrega una sección al reporte. Solo permitido en estado DRAFT. |
| isDraft() | Boolean | Public | Indica si el reporte está en estado borrador. |
| isPublished() | Boolean | Public | Indica si el reporte ya fue publicado. |

---

**Value Object: ReportSection**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| ReportSection | Value Object | Sección individual dentro de un reporte técnico. Inmutable. |

**Attributes**

| Nombre | Tipo de dato | Visibilidad | Descripción |
|--------|-------------|-------------|-------------|
| title | String | Private | Título de la sección. |
| content | String | Private | Contenido textual de la sección. |
| sectionType | SectionType (enum) | Private | Tipo: MONITORING_SUMMARY, ALERT_REVIEW, RECOMMENDATIONS, OBSERVATIONS. |

---

**Enum: ReportStatus**

| Código | Descripción |
|--------|-------------|
| DRAFT | Reporte en borrador, editable. |
| PUBLISHED | Reporte publicado, inmutable. |

**Enum: SectionType**

| Código | Descripción |
|--------|-------------|
| MONITORING_SUMMARY | Resumen de datos de monitoreo. |
| ALERT_REVIEW | Revisión de alertas del período. |
| RECOMMENDATIONS | Recomendaciones agronómicas. |
| OBSERVATIONS | Observaciones de campo. |

---

**Domain Services**

| Nombre | Descripción |
|--------|-------------|
| CropHealthEvaluationService | Servicio de dominio que implementa la lógica de priorización worst-wins para consolidar el estado de salud a nivel zona y plantación. Resuelve el pain point de conflicto entre parámetros. |
| ZonePriorityRankingService | Servicio de dominio que ordena zonas y plantaciones por criticidad para ayudar al agrónomo a priorizar su atención técnica. |

---

**Repository Interfaces (Domain contracts)**

| Nombre | Descripción |
|--------|-------------|
| CropHealthSnapshotRepository | Contrato para persistir y consultar snapshots de estado de salud por zona y plantación. |
| PlantationOverviewRepository | Contrato para persistir y consultar vistas consolidadas de plantaciones. |
| SensorTimeSeriesRepository | Contrato para consultar series temporales de variables monitoreadas por zona y período. |
| TechnicalReportRepository | Contrato para persistir y consultar reportes técnicos por plantación y estado. |

#### 4.2.5.2. Interface Layer

Esta capa es responsable de la recepción y formato de peticiones/respuestas externas (API REST), validación básica del formato y los datos de entrada, manejo de errores a nivel de API y delegación de la lógica de negocio a la capa de Aplicación. Expone los endpoints consumidos por la aplicación móvil del dueño del cultivo y la plataforma web del ingeniero agrónomo.

---

**Controller 1: CropHealthController**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| CropHealthController | Controller | Controlador para los endpoints relacionados con la consulta del estado de salud del cultivo a nivel de zona y plantación. Consume datos consolidados desde los snapshots generados por el sistema. |

**Attributes**

| Nombre | Tipo de dato | Visibilidad | Descripción |
|--------|-------------|-------------|-------------|
| cropHealthService | CropHealthService | Private | Servicio de la capa de Aplicación para lógica de consulta de estado de salud. |
| snapshotMapper | CropHealthSnapshotMapper | Private | Mapper para convertir entre entidades de dominio y DTOs de respuesta. |

**Endpoints**

| Ruta | Método | Descripción |
|------|--------|-------------|
| /api/monitoring/plantations/{plantationId}/health | GET | Retorna el estado general de salud de una plantación con el resumen de todas sus zonas. |
| /api/monitoring/plantations/{plantationId}/zones/{zoneId}/health | GET | Retorna el estado de salud detallado de una zona de monitoreo específica, incluyendo el resumen de cada parámetro. |
| /api/monitoring/plantations/{plantationId}/zones/priority | GET | Retorna las zonas de una plantación ordenadas por criticidad descendente para priorización del agrónomo. |
| /api/monitoring/plantations/{plantationId}/zones/compare | GET | Retorna la comparación de condiciones entre múltiples zonas de una misma plantación. |

---

**Controller 2: PlantationOverviewController**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| PlantationOverviewController | Controller | Controlador para los endpoints de vista consolidada de plantaciones. Provee el panel general para el agrónomo y el resumen para el dueño del cultivo. |

**Attributes**

| Nombre | Tipo de dato | Visibilidad | Descripción |
|--------|-------------|-------------|-------------|
| plantationOverviewService | PlantationOverviewService | Private | Servicio de la capa de Aplicación para lógica de consulta de vistas de plantación. |
| overviewMapper | PlantationOverviewMapper | Private | Mapper para convertir entre entidades de dominio y DTOs de respuesta. |

**Endpoints**

| Ruta | Método | Descripción |
|------|--------|-------------|
| /api/monitoring/plantations/overview | GET | Retorna el resumen consolidado de todas las plantaciones asignadas al usuario autenticado (agrónomo o dueño del cultivo). |
| /api/monitoring/plantations/{plantationId}/overview | GET | Retorna el detalle de la vista consolidada de una plantación específica, incluyendo conteo de zonas por estado y última actualización. |
| /api/monitoring/plantations/{plantationId}/last-update | GET | Retorna la fecha y hora de la última actualización de datos de una plantación. |

---

**Controller 3: SensorTimeSeriesController**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| SensorTimeSeriesController | Controller | Controlador para los endpoints de consulta de historial y tendencias de variables monitoreadas por zona. Permite al agrónomo analizar el comportamiento del cultivo en el tiempo. |

**Attributes**

| Nombre | Tipo de dato | Visibilidad | Descripción |
|--------|-------------|-------------|-------------|
| timeSeriesService | SensorTimeSeriesService | Private | Servicio de la capa de Aplicación para lógica de consulta de series temporales. |
| timeSeriesMapper | SensorTimeSeriesMapper | Private | Mapper para convertir entre entidades de dominio y DTOs de respuesta. |

**Endpoints**

| Ruta | Método | Descripción |
|------|--------|-------------|
| /api/monitoring/zones/{zoneId}/time-series | GET | Retorna la serie temporal de una variable específica en una zona para un rango de fechas dado. Requiere query params: parameterType, startDate, endDate. |
| /api/monitoring/zones/{zoneId}/time-series/trend | GET | Retorna la tendencia calculada (RISING, STABLE, FALLING) de una variable en una zona para un período. Requiere query params: parameterType, startDate, endDate. |
| /api/monitoring/zones/{zoneId}/time-series/summary | GET | Retorna el resumen estadístico (promedio, máximo, mínimo, anomalías) de una variable en una zona para un período. Requiere query params: parameterType, startDate, endDate. |
| /api/monitoring/zones/{zoneId}/variables | GET | Retorna las variables monitoreadas disponibles en una zona con sus valores actuales. |

---

**Controller 4: TechnicalReportController**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| TechnicalReportController | Controller | Controlador para los endpoints de generación, publicación, consulta y exportación de reportes técnicos del agrónomo. |

**Attributes**

| Nombre | Tipo de dato | Visibilidad | Descripción |
|--------|-------------|-------------|-------------|
| technicalReportService | TechnicalReportService | Private | Servicio de la capa de Aplicación para lógica de gestión de reportes técnicos. |
| reportMapper | TechnicalReportMapper | Private | Mapper para convertir entre entidades de dominio y DTOs de respuesta. |

**Endpoints**

| Ruta | Método | Descripción |
|------|--------|-------------|
| /api/monitoring/plantations/{plantationId}/reports | POST | Genera un nuevo borrador de reporte técnico para una plantación. El sistema incluye automáticamente los snapshots y alertas del período. |
| /api/monitoring/plantations/{plantationId}/reports | GET | Retorna la lista de reportes técnicos generados para una plantación, con filtro opcional por estado (DRAFT, PUBLISHED). |
| /api/monitoring/reports/{reportId} | GET | Retorna el detalle completo de un reporte técnico específico. |
| /api/monitoring/reports/{reportId}/sections | POST | Agrega una sección al reporte. Solo permitido si el reporte está en estado DRAFT. |
| /api/monitoring/reports/{reportId}/publish | PUT | Publica un reporte técnico, cambiando su estado de DRAFT a PUBLISHED. |
| /api/monitoring/reports/{reportId}/export | GET | Exporta un reporte técnico en formato descargable (PDF/CSV). |

---

**Controller 5: AlertDashboardController**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| AlertDashboardController | Controller | Controlador para los endpoints de consulta de alertas activas e historial de alertas desde la perspectiva del dashboard de monitoreo. Consume datos del BC-03 (Alert & Notification). |

**Attributes**

| Nombre | Tipo de dato | Visibilidad | Descripción |
|--------|-------------|-------------|-------------|
| alertDashboardService | AlertDashboardService | Private | Servicio de la capa de Aplicación para lógica de consulta de alertas en el contexto del dashboard. |
| alertMapper | AlertDashboardMapper | Private | Mapper para convertir entre datos de alertas y DTOs de respuesta. |

**Endpoints**

| Ruta | Método | Descripción |
|------|--------|-------------|
| /api/monitoring/plantations/{plantationId}/alerts/active | GET | Retorna las alertas activas de una plantación, agrupadas por zona y severidad. |
| /api/monitoring/plantations/{plantationId}/alerts/history | GET | Retorna el historial de alertas de una plantación con filtros opcionales por zona, severidad y rango de fechas. |

---

**Controller 6: RecommendationFeedController**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| RecommendationFeedController | Controller | Controlador para los endpoints de consulta de recomendaciones publicadas desde la perspectiva del dashboard. Consume datos del BC-04 (Agronomic Recommendation). |

**Attributes**

| Nombre | Tipo de dato | Visibilidad | Descripción |
|--------|-------------|-------------|-------------|
| recommendationFeedService | RecommendationFeedService | Private | Servicio de la capa de Aplicación para lógica de consulta de recomendaciones publicadas. |
| recommendationMapper | RecommendationFeedMapper | Private | Mapper para convertir entre datos de recomendaciones y DTOs de respuesta. |

**Endpoints**

| Ruta | Método | Descripción |
|------|--------|-------------|
| /api/monitoring/plantations/{plantationId}/recommendations | GET | Retorna las recomendaciones publicadas para una plantación, ordenadas cronológicamente. |
| /api/monitoring/plantations/{plantationId}/recommendations/{recommendationId} | GET | Retorna el detalle de una recomendación específica. |

---

**DTOs**

| Nombre | Descripción |
|--------|-------------|
| CropHealthSnapshotResponseDto | { id: UUID, zoneId: UUID, plantationId: UUID, overallStatus: String, parameterSummaries: List\<ParameterSummaryDto\>, dominantRiskFactor: String, evaluatedAt: DateTime, activeAlertCount: Integer } |
| ParameterSummaryDto | { parameterType: String, currentValue: Double, unit: String, status: String, thresholdMin: Double, thresholdMax: Double } |
| PlantationOverviewResponseDto | { id: UUID, plantationId: UUID, plantationName: String, totalZones: Integer, criticalZones: Integer, atRiskZones: Integer, optimalZones: Integer, overallStatus: String, lastUpdatedAt: DateTime } |
| PlantationOverviewListResponseDto | { plantations: List\<PlantationOverviewResponseDto\> } |
| ZonePriorityResponseDto | { zones: List\<CropHealthSnapshotResponseDto\> } |
| ZoneComparisonResponseDto | { plantationId: UUID, zones: List\<CropHealthSnapshotResponseDto\>, comparedAt: DateTime } |
| LastUpdateResponseDto | { plantationId: UUID, lastUpdatedAt: DateTime, hasDataGap: Boolean } |
| TimeSeriesRequestParams | { parameterType: String, startDate: DateTime, endDate: DateTime } |
| TimeSeriesResponseDto | { zoneId: UUID, parameterType: String, startDate: DateTime, endDate: DateTime, dataPoints: List\<TimeSeriesDataPointDto\>, trendDirection: String } |
| TimeSeriesDataPointDto | { timestamp: DateTime, value: Double, status: String } |
| TimeSeriesTrendResponseDto | { zoneId: UUID, parameterType: String, trendDirection: String, startDate: DateTime, endDate: DateTime } |
| TimeSeriesSummaryResponseDto | { zoneId: UUID, parameterType: String, averageValue: Double, maxValue: Double, minValue: Double, hasAnomalies: Boolean, startDate: DateTime, endDate: DateTime } |
| ZoneVariablesResponseDto | { zoneId: UUID, variables: List\<ParameterSummaryDto\> } |
| GenerateReportRequestDto | { title: String, startDate: DateTime, endDate: DateTime } |
| AddReportSectionRequestDto | { title: String, content: String, sectionType: String } |
| TechnicalReportResponseDto | { id: UUID, plantationId: UUID, authorId: UUID, title: String, summary: String, sections: List\<ReportSectionDto\>, status: String, generatedAt: DateTime, publishedAt: DateTime } |
| TechnicalReportListResponseDto | { reports: List\<TechnicalReportSummaryDto\> } |
| TechnicalReportSummaryDto | { id: UUID, title: String, status: String, generatedAt: DateTime, publishedAt: DateTime } |
| ReportSectionDto | { title: String, content: String, sectionType: String } |
| ActiveAlertResponseDto | { alertId: UUID, zoneId: UUID, zoneName: String, parameterType: String, severity: String, message: String, triggeredAt: DateTime } |
| ActiveAlertListResponseDto | { plantationId: UUID, alerts: List\<ActiveAlertResponseDto\>, totalCount: Integer } |
| AlertHistoryResponseDto | { plantationId: UUID, alerts: List\<ActiveAlertResponseDto\>, totalCount: Integer, startDate: DateTime, endDate: DateTime } |
| RecommendationFeedResponseDto | { recommendationId: UUID, plantationId: UUID, zoneId: UUID, type: String, content: String, author: String, publishedAt: DateTime } |
| RecommendationFeedListResponseDto | { plantationId: UUID, recommendations: List\<RecommendationFeedResponseDto\>, totalCount: Integer } |

---

#### 4.2.5.3. Application Layer

En la capa de Application Layer se ubican los servicios que orquestan los casos de uso del bounded context Crop Monitoring Dashboard. Estos servicios coordinan la lógica de negocio delegando a las entidades y servicios de dominio, gestionan transacciones, y actúan como intermediarios entre la capa de Interface y la capa de Domain. No contienen lógica de negocio propia, sino que orquestan las operaciones necesarias para satisfacer cada caso de uso.

---

**Service 1: CropHealthService**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| CropHealthService | Application Service | Servicio de aplicación responsable de los casos de uso relacionados con la consulta y evaluación del estado de salud del cultivo a nivel de zona y plantación. Coordina la consolidación de snapshots y la priorización de zonas. |

**Dependencies**

| Nombre | Tipo de Objeto | Visibilidad | Descripción |
|--------|---------------|-------------|-------------|
| cropHealthSnapshotRepository | CropHealthSnapshotRepository | Private | Acceso a la persistencia de snapshots de salud por zona. |
| cropHealthEvaluationService | CropHealthEvaluationService | Private | Servicio de dominio para aplicar la lógica de priorización worst-wins. |
| zonePriorityRankingService | ZonePriorityRankingService | Private | Servicio de dominio para ordenar zonas por criticidad. |
| snapshotMapper | CropHealthSnapshotMapper | Private | Mapper para convertir entre entidades de dominio y DTOs de respuesta. |

**Methods**

| Nombre | Tipo de retorno | Visibilidad | Descripción |
|--------|----------------|-------------|-------------|
| getPlantationHealth(GetPlantationHealthQuery query) | CropHealthSnapshotResponseDto | Public | Obtiene el estado de salud consolidado de una plantación a partir de los snapshots de sus zonas. |
| getZoneHealth(GetZoneHealthQuery query) | CropHealthSnapshotResponseDto | Public | Obtiene el estado de salud detallado de una zona específica con el resumen de cada parámetro. |
| getZonesByPriority(GetZonesByPriorityQuery query) | ZonePriorityResponseDto | Public | Retorna las zonas de una plantación ordenadas por criticidad descendente. |
| compareZones(CompareZonesQuery query) | ZoneComparisonResponseDto | Public | Compara las condiciones de múltiples zonas de una misma plantación. |

---

**Service 2: PlantationOverviewService**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| PlantationOverviewService | Application Service | Servicio de aplicación responsable de los casos de uso de consulta de vistas consolidadas de plantaciones. Provee el panel general para el agrónomo y el resumen para el dueño del cultivo. |

**Dependencies**

| Nombre | Tipo de Objeto | Visibilidad | Descripción |
|--------|---------------|-------------|-------------|
| plantationOverviewRepository | PlantationOverviewRepository | Private | Acceso a la persistencia de vistas consolidadas de plantaciones. |
| cropHealthSnapshotRepository | CropHealthSnapshotRepository | Private | Acceso a snapshots para reconstruir la vista si es necesario. |
| overviewMapper | PlantationOverviewMapper | Private | Mapper para convertir entre entidades de dominio y DTOs de respuesta. |

**Methods**

| Nombre | Tipo de retorno | Visibilidad | Descripción |
|--------|----------------|-------------|-------------|
| getAllPlantationsOverview(GetAllPlantationsOverviewQuery query) | PlantationOverviewListResponseDto | Public | Retorna el resumen consolidado de todas las plantaciones asignadas al usuario autenticado. |
| getPlantationOverview(GetPlantationOverviewQuery query) | PlantationOverviewResponseDto | Public | Retorna el detalle de la vista consolidada de una plantación específica. |
| getLastUpdate(GetLastUpdateQuery query) | LastUpdateResponseDto | Public | Retorna la fecha y hora de la última actualización de datos de una plantación e indica si existe brecha de datos. |

---

**Service 3: SensorTimeSeriesService**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| SensorTimeSeriesService | Application Service | Servicio de aplicación responsable de los casos de uso de consulta de historial, tendencias y resumen estadístico de variables monitoreadas por zona. Permite al agrónomo analizar el comportamiento del cultivo en el tiempo. |

**Dependencies**

| Nombre | Tipo de Objeto | Visibilidad | Descripción |
|--------|---------------|-------------|-------------|
| sensorTimeSeriesRepository | SensorTimeSeriesRepository | Private | Acceso a la persistencia de series temporales de variables monitoreadas. |
| cropHealthSnapshotRepository | CropHealthSnapshotRepository | Private | Acceso a snapshots para obtener los valores actuales de las variables de una zona. |
| timeSeriesMapper | SensorTimeSeriesMapper | Private | Mapper para convertir entre entidades de dominio y DTOs de respuesta. |

**Methods**

| Nombre | Tipo de retorno | Visibilidad | Descripción |
|--------|----------------|-------------|-------------|
| getTimeSeries(GetTimeSeriesQuery query) | TimeSeriesResponseDto | Public | Retorna la serie temporal de una variable específica en una zona para un rango de fechas. |
| getTrend(GetTrendQuery query) | TimeSeriesTrendResponseDto | Public | Retorna la tendencia calculada de una variable en una zona para un período. |
| getTimeSeriesSummary(GetTimeSeriesSummaryQuery query) | TimeSeriesSummaryResponseDto | Public | Retorna el resumen estadístico (promedio, máximo, mínimo, anomalías) de una variable en una zona. |
| getZoneVariables(GetZoneVariablesQuery query) | ZoneVariablesResponseDto | Public | Retorna las variables monitoreadas disponibles en una zona con sus valores actuales. |

---

**Service 4: TechnicalReportService**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| TechnicalReportService | Application Service | Servicio de aplicación responsable de los casos de uso de generación, edición, publicación, consulta y exportación de reportes técnicos. Orquesta la creación del borrador incorporando datos de monitoreo y alertas del período. |

**Dependencies**

| Nombre | Tipo de Objeto | Visibilidad | Descripción |
|--------|---------------|-------------|-------------|
| technicalReportRepository | TechnicalReportRepository | Private | Acceso a la persistencia de reportes técnicos. |
| cropHealthSnapshotRepository | CropHealthSnapshotRepository | Private | Acceso a snapshots para incluir datos de monitoreo en el reporte. |
| alertDashboardService | AlertDashboardService | Private | Servicio de consulta de alertas para incluir historial en el reporte. |
| reportMapper | TechnicalReportMapper | Private | Mapper para convertir entre entidades de dominio y DTOs de respuesta. |

**Methods**

| Nombre | Tipo de retorno | Visibilidad | Descripción |
|--------|----------------|-------------|-------------|
| generateReportDraft(GenerateReportDraftCommand command) | TechnicalReportResponseDto | Public | Genera un nuevo borrador de reporte técnico para una plantación, incorporando automáticamente snapshots y alertas del período solicitado. |
| addSection(AddReportSectionCommand command) | TechnicalReportResponseDto | Public | Agrega una sección al reporte. Lanza excepción si el reporte no está en estado DRAFT. |
| publishReport(PublishReportCommand command) | TechnicalReportResponseDto | Public | Publica un reporte técnico cambiando su estado de DRAFT a PUBLISHED. |
| getReportById(GetReportByIdQuery query) | TechnicalReportResponseDto | Public | Retorna el detalle completo de un reporte técnico específico. |
| getReportsByPlantation(GetReportsByPlantationQuery query) | TechnicalReportListResponseDto | Public | Retorna la lista de reportes técnicos de una plantación con filtro opcional por estado. |
| exportReport(ExportReportCommand command) | byte[] | Public | Exporta un reporte técnico en formato descargable (PDF/CSV). |

---

**Service 5: AlertDashboardService**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| AlertDashboardService | Application Service | Servicio de aplicación responsable de los casos de uso de consulta de alertas activas e historial de alertas en el contexto del dashboard de monitoreo. Actúa como consumidor del BC-03 (Alert & Notification) a través de una interfaz anticorrupción. |

**Dependencies**

| Nombre | Tipo de Objeto | Visibilidad | Descripción |
|--------|---------------|-------------|-------------|
| alertQueryClient | AlertQueryClient | Private | Cliente de integración (ACL) para consultar datos de alertas del BC-03. |
| alertMapper | AlertDashboardMapper | Private | Mapper para convertir entre datos externos de alertas y DTOs de respuesta del dashboard. |

**Methods**

| Nombre | Tipo de retorno | Visibilidad | Descripción |
|--------|----------------|-------------|-------------|
| getActiveAlerts(GetActiveAlertsQuery query) | ActiveAlertListResponseDto | Public | Retorna las alertas activas de una plantación, agrupadas por zona y severidad. |
| getAlertHistory(GetAlertHistoryQuery query) | AlertHistoryResponseDto | Public | Retorna el historial de alertas de una plantación con filtros por zona, severidad y rango de fechas. |

---

**Service 6: RecommendationFeedService**

| Nombre | Categoría | Descripción |
|--------|-----------|-------------|
| RecommendationFeedService | Application Service | Servicio de aplicación responsable de los casos de uso de consulta de recomendaciones publicadas en el contexto del dashboard. Actúa como consumidor del BC-04 (Agronomic Recommendation) a través de una interfaz anticorrupción. |

**Dependencies**

| Nombre | Tipo de Objeto | Visibilidad | Descripción |
|--------|---------------|-------------|-------------|
| recommendationQueryClient | RecommendationQueryClient | Private | Cliente de integración (ACL) para consultar datos de recomendaciones del BC-04. |
| recommendationMapper | RecommendationFeedMapper | Private | Mapper para convertir entre datos externos de recomendaciones y DTOs de respuesta del dashboard. |

**Methods**

| Nombre | Tipo de retorno | Visibilidad | Descripción |
|--------|----------------|-------------|-------------|
| getRecommendationsByPlantation(GetRecommendationsByPlantationQuery query) | RecommendationFeedListResponseDto | Public | Retorna las recomendaciones publicadas para una plantación, ordenadas cronológicamente. |
| getRecommendationDetail(GetRecommendationDetailQuery query) | RecommendationFeedResponseDto | Public | Retorna el detalle de una recomendación específica. |

---

**Commands**

| Nombre | Atributos | Descripción |
|--------|-----------|-------------|
| GenerateReportDraftCommand | plantationId: UUID, authorId: UUID, title: String, startDate: LocalDateTime, endDate: LocalDateTime | Comando para generar un nuevo borrador de reporte técnico. |
| AddReportSectionCommand | reportId: UUID, title: String, content: String, sectionType: SectionType | Comando para agregar una sección a un reporte en borrador. |
| PublishReportCommand | reportId: UUID, authorId: UUID | Comando para publicar un reporte técnico. |
| ExportReportCommand | reportId: UUID, format: String | Comando para exportar un reporte en formato descargable. |

**Queries**

| Nombre | Atributos | Descripción |
|--------|-----------|-------------|
| GetPlantationHealthQuery | plantationId: UUID | Consulta del estado de salud consolidado de una plantación. |
| GetZoneHealthQuery | plantationId: UUID, zoneId: UUID | Consulta del estado de salud detallado de una zona. |
| GetZonesByPriorityQuery | plantationId: UUID | Consulta de zonas ordenadas por criticidad. |
| CompareZonesQuery | plantationId: UUID, zoneIds: List\<UUID\> | Consulta de comparación entre zonas específicas. |
| GetAllPlantationsOverviewQuery | userId: UUID, role: String | Consulta del resumen de todas las plantaciones asignadas al usuario. |
| GetPlantationOverviewQuery | plantationId: UUID | Consulta del detalle de la vista consolidada de una plantación. |
| GetLastUpdateQuery | plantationId: UUID | Consulta de la última actualización de datos de una plantación. |
| GetTimeSeriesQuery | zoneId: UUID, parameterType: ParameterType, startDate: LocalDateTime, endDate: LocalDateTime | Consulta de serie temporal de una variable en una zona. |
| GetTrendQuery | zoneId: UUID, parameterType: ParameterType, startDate: LocalDateTime, endDate: LocalDateTime | Consulta de tendencia de una variable en una zona. |
| GetTimeSeriesSummaryQuery | zoneId: UUID, parameterType: ParameterType, startDate: LocalDateTime, endDate: LocalDateTime | Consulta de resumen estadístico de una variable en una zona. |
| GetZoneVariablesQuery | zoneId: UUID | Consulta de las variables monitoreadas en una zona. |
| GetReportByIdQuery | reportId: UUID | Consulta de un reporte técnico por su ID. |
| GetReportsByPlantationQuery | plantationId: UUID, status: ReportStatus (opcional) | Consulta de reportes de una plantación con filtro opcional por estado. |
| GetActiveAlertsQuery | plantationId: UUID | Consulta de alertas activas de una plantación. |
| GetAlertHistoryQuery | plantationId: UUID, zoneId: UUID (opcional), severity: String (opcional), startDate: LocalDateTime (opcional), endDate: LocalDateTime (opcional) | Consulta de historial de alertas con filtros. |
| GetRecommendationsByPlantationQuery | plantationId: UUID | Consulta de recomendaciones publicadas para una plantación. |
| GetRecommendationDetailQuery | recommendationId: UUID | Consulta del detalle de una recomendación específica. |

#### 4.2.5.4. Infrastructure Layer

En la capa de Infrastructure Layer se encuentran las implementaciones concretas de los contratos definidos en las capas de dominio y aplicación. Incluye los repositorios JPA que manejan la persistencia de las entidades en la base de datos, los clientes de integración con otros bounded contexts (Anti-Corruption Layer) y los servicios técnicos de exportación. Esta capa aísla los detalles tecnológicos del dominio, permitiendo que la lógica de negocio permanezca independiente de frameworks y proveedores específicos.

---

**JpaCropHealthSnapshotRepositoryImpl**

| Nombre | Categoría | Implementa | Descripción |
|--------|-----------|------------|-------------|
| JpaCropHealthSnapshotRepositoryImpl | Repository Implementation | CropHealthSnapshotRepository | Implementación concreta de la interfaz CropHealthSnapshotRepository utilizando JPA y Spring Data JPA. Maneja el mapeo entre el agregado de dominio CropHealthSnapshot y la base de datos relacional, incluyendo la persistencia de los value objects ParameterSummary como entidades embebidas. |

**Funcionalidad clave**

- Busca y carga agregados CropHealthSnapshot por ID, zoneId y plantationId.
- Guarda (inserta/actualiza) agregados CropHealthSnapshot con sus ParameterSummary asociados.
- Consulta los snapshots más recientes por plantación para construir la vista consolidada.
- Filtra snapshots por estado (OPTIMAL, AT_RISK, CRITICAL) y rango de fechas.
- Obtiene el snapshot más reciente de una zona específica.
- Cuenta alertas activas asociadas a un snapshot por zona.

---

**JpaPlantationOverviewRepositoryImpl**

| Nombre | Categoría | Implementa | Descripción |
|--------|-----------|------------|-------------|
| JpaPlantationOverviewRepositoryImpl | Repository Implementation | PlantationOverviewRepository | Implementación concreta de la interfaz PlantationOverviewRepository utilizando JPA y Spring Data JPA. Maneja el mapeo entre el agregado de dominio PlantationOverview y la base de datos, incluyendo la relación con los CropHealthSnapshot de cada zona. |

**Funcionalidad clave**

- Busca y carga agregados PlantationOverview por ID y plantationId.
- Guarda (inserta/actualiza) agregados PlantationOverview con sus conteos de zonas por estado.
- Lista todas las plantaciones asignadas a un usuario por su userId y rol.
- Consulta la fecha de última actualización de datos por plantación.
- Verifica si existe brecha de datos (zonas sin actualización en más de 24 horas).

---

**JpaSensorTimeSeriesRepositoryImpl**

| Nombre | Categoría | Implementa | Descripción |
|--------|-----------|------------|-------------|
| JpaSensorTimeSeriesRepositoryImpl | Repository Implementation | SensorTimeSeriesRepository | Implementación concreta de la interfaz SensorTimeSeriesRepository utilizando JPA y Spring Data JPA. Maneja la consulta de series temporales de variables monitoreadas, optimizada para rangos de fechas y agregaciones estadísticas. |

**Funcionalidad clave**

- Consulta series temporales de una variable por zona y rango de fechas.
- Ejecuta agregaciones estadísticas (promedio, máximo, mínimo) directamente en base de datos para optimizar rendimiento.
- Detecta anomalías (valores fuera de umbrales) en un período determinado.
- Lista las variables monitoreadas disponibles en una zona con sus últimos valores registrados.
- Soporta paginación para series temporales extensas.

---

**JpaTechnicalReportRepositoryImpl**

| Nombre | Categoría | Implementa | Descripción |
|--------|-----------|------------|-------------|
| JpaTechnicalReportRepositoryImpl | Repository Implementation | TechnicalReportRepository | Implementación concreta de la interfaz TechnicalReportRepository utilizando JPA y Spring Data JPA. Maneja el mapeo entre el agregado de dominio TechnicalReport y la base de datos, incluyendo la persistencia de las secciones del reporte como entidades embebidas. |

**Funcionalidad clave**

- Busca y carga agregados TechnicalReport por ID, plantationId y authorId.
- Guarda (inserta/actualiza) agregados TechnicalReport con sus ReportSection asociadas.
- Lista reportes por plantación con filtro opcional por estado (DRAFT, PUBLISHED).
- Ordena reportes por fecha de generación o publicación.
- Verifica la existencia de un reporte por ID antes de operaciones de edición o publicación.

---

**AlertQueryClientImpl**

| Nombre | Categoría | Implementa | Descripción |
|--------|-----------|------------|-------------|
| AlertQueryClientImpl | Anti-Corruption Layer Client | AlertQueryClient | Implementación concreta del cliente de integración con el BC-03 (Alert & Notification). Traduce las respuestas del contexto externo de alertas al modelo interno del dashboard de monitoreo, protegiendo al BC-05 de cambios en el modelo de dominio del BC-03. |

**Funcionalidad clave**

- Consulta alertas activas del BC-03 por plantationId y las transforma al modelo interno del dashboard.
- Consulta historial de alertas del BC-03 con filtros por zona, severidad y rango de fechas.
- Mapea los niveles de severidad del BC-03 (CRITICAL, WARNING, INFORMATIONAL) al formato esperado por los DTOs del dashboard.
- Maneja errores de comunicación con el BC-03, retornando respuestas vacías con indicador de error en caso de indisponibilidad.
- Implementa caché local para reducir la frecuencia de llamadas al BC-03 en consultas repetitivas.

---

**RecommendationQueryClientImpl**

| Nombre | Categoría | Implementa | Descripción |
|--------|-----------|------------|-------------|
| RecommendationQueryClientImpl | Anti-Corruption Layer Client | RecommendationQueryClient | Implementación concreta del cliente de integración con el BC-04 (Agronomic Recommendation). Traduce las respuestas del contexto externo de recomendaciones al modelo interno del dashboard, protegiendo al BC-05 de cambios en el modelo de dominio del BC-04. |

**Funcionalidad clave**

- Consulta recomendaciones publicadas del BC-04 por plantationId y las transforma al modelo interno del dashboard.
- Consulta el detalle de una recomendación específica por recommendationId.
- Mapea los tipos de recomendación del BC-04 (AI_GENERATED, MANUAL) y su estado de aprobación al formato esperado por los DTOs del feed.
- Maneja errores de comunicación con el BC-04, retornando respuestas vacías con indicador de error en caso de indisponibilidad.
- Filtra únicamente recomendaciones en estado PUBLISHED, descartando borradores o pendientes de aprobación del BC-04.

---

**ReportExportServiceImpl**

| Nombre | Categoría | Implementa | Descripción |
|--------|-----------|------------|-------------|
| ReportExportServiceImpl | Technical Service | ReportExportService | Implementación concreta del servicio técnico de exportación de reportes. Genera archivos descargables en formato PDF y CSV a partir de los datos del reporte técnico almacenado en el sistema. |

**Funcionalidad clave**

- Genera documentos PDF a partir de un TechnicalReport, incluyendo secciones, resumen ejecutivo y datos de monitoreo referenciados.
- Genera archivos CSV con los datos tabulares del reporte (lecturas, alertas, resumen por zona).
- Aplica plantillas de formato institucional para los reportes PDF exportados.
- Maneja la inclusión de gráficos de tendencias como imágenes embebidas en el PDF cuando están disponibles.

---

**TrendCalculationServiceImpl**

| Nombre | Categoría | Implementa | Descripción |
|--------|-----------|------------|-------------|
| TrendCalculationServiceImpl | Technical Service | TrendCalculationService | Implementación concreta del servicio técnico de cálculo de tendencias. Aplica algoritmos de regresión lineal simple sobre los data points de una serie temporal para determinar la dirección de la tendencia (RISING, STABLE, FALLING). |

**Funcionalidad clave**

- Calcula la pendiente de regresión lineal sobre un conjunto de TimeSeriesDataPoint.
- Clasifica la tendencia según umbrales configurables: pendiente positiva significativa (RISING), pendiente negativa significativa (FALLING), o dentro del margen de estabilidad (STABLE).
- Maneja series con datos insuficientes (menos de 3 puntos), retornando STABLE como valor por defecto con indicador de baja confianza.
- Detecta anomalías estadísticas (valores atípicos) que podrían distorsionar el cálculo de tendencia.

