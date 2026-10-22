# ⚡ Pruebas No Funcionales con Apache JMeter — Guía y Calibración para Azure for Students

## 1. Herramienta Estandarizada de Pruebas: Apache JMeter
De conformidad con las directrices institucionales del proyecto ([AGENTS.md](../../AGENTS.md)), la herramienta oficial y estandarizada para el diseño y ejecución de pruebas no funcionales (carga, concurrencia, rendimiento y estrés) es **Apache JMeter**.

### ¿Por qué Apache JMeter?
- **Estándar de la Industria:** Permite modelar protocolos HTTP/HTTPS, manejo de cookies de sesión, temporizadores gaussianos (*Think Time*) y extracción dinámica de tokens antifalsificación (`__RequestVerificationToken`).
- **Compatibilidad con Azure:** Los planes de prueba `.jmx` generados son 100% compatibles tanto con ejecución local en línea de comandos (CLI) como con el servicio en la nube **Azure Load Testing**.
- **Generación de Reportes Ejecutivos:** Capacidad nativa de generar dashboards interactivos en HTML con métricas percentiles ($p_{90}$, $p_{95}$, $p_{99}$), throughput y tasas de error.

---

## 2. Contexto de Infraestructura: Azure for Students (Nivel Gratuito / Básico)
Las pruebas han sido **calibradas técnicamente** para operar de manera realista y sostenible sobre las capacidades del plan educativo de Microsoft Azure:

- **Azure App Service (Plan F1 Free o B1 Basic):**
  - F1 Free: Límite diario estricto de **60 minutos de CPU/día** y **1 GB de RAM compartida**.
  - B1 Basic: 1 vCPU dedicada y 1.75 GB de RAM.
  - *Mitigación:* Pruebas con *Ramp-up* progresivo y tiempos de pausa que no consumen prematuramente la cuota diaria de cómputo ni provocan bloqueos por `HTTP 429 / 403 Quota Exceeded`.
- **Azure SQL Database (Free Tier / Nivel Básico de 5 DTUs):**
  - Capacidad limitada de transacciones por segundo.
  - *Mitigación:* Concurrencia modelada por grupos de usuarios (10 a 25 hilos) que evita la saturación artificial del pool de conexiones o errores `Timeout Expired`.
- **Servicio de Correo (Azure Communication Services):**
  - Para pruebas de carga masiva de autenticación y registro se utiliza `DevEmailService` en memoria/logger para no incurrir en costos ni bloqueos por cuota de envío SMTP.

---

## 3. Matriz de Escenarios Calibrados vs. Escenarios Teóricos

| Escenario / Código | Enfoque Teórico Previo (Inviable en Free Tier) | Enfoque Calibrado (Azure for Students) | Endpoint HTTP y Método Real | Justificación y Valor para el Proyecto UPIICSA |
| :--- | :--- | :--- | :--- | :--- |
| **JMX-01<br>Escaneo QR en Salón** | 50 hilos consumiendo `/Asistencia/RegistrarQr` (ruta inexistente). | **15 alumnos concurrentes** en una ventana de 15 segundos con validación de token. | `GET /Asistencia/ValidarQr?token={token}` | **Realidad Académica:** En UPIICSA una tutoría docente tiene cupo máximo de 10 alumnos y una grupal/entre pares ronda 15-20 alumnos escaneando el QR proyectado. Valida que el servidor asiente las asistencias en segundos sin colisiones ni bloqueos de concurrencia en SQL Server.<br>**Métrica:** $p_{95} < 800$ ms, éxito 100%. |
| **JMX-02<br>Catálogo de Tutorías** | 100 hilos disparando peticiones continuas sin pausa. | **25 usuarios concurrentes** con *Ramp-up* de 30s y *Think Time* de 1-2s. | `GET /Catalogo/Index`<br>`[AllowAnonymous]` | **Realidad Académica:** Simula el periodo de alta consulta de oferta de tutorías al inicio del semestre. El tiempo de lectura emula al estudiante revisando tarjetas y aplicando filtros de materia/carrera.<br>**Métrica:** Throughput $> 15$ req/s, $p_{95} < 700$ ms, 0% errores HTTP 5xx. |
| **JMX-03<br>Autenticación (Login)** | 50 logins simultáneos directos por POST. | **12 inicios de sesión concurrentes** resolviendo el token antifalsificación. | `GET /Account/Login`<br>↓<br>`POST /Account/Login` | **Realidad Técnica:** ASP.NET Core Identity usa PBKDF2 para hash criptográfico de contraseñas, lo cual es intensivo en CPU. 12 logins simultáneos evalúa la capacidad de respuesta de la CPU compartida de Azure sin congelar el servidor.<br>**Métrica:** $p_{95} < 1200$ ms, sin saturación del pool de conexiones. |
| **JMX-04<br>Reporte PDF (QuestPDF)** | Hilos concurrentes masivos a ruta inexistente. | **3 descargas simultáneas** autenticadas con rol Coordinador. | `GET /Reportes/ExportarMetricasPdf`<br>`[Authorize(Roles="Coordinador")]` | **Realidad Operativa:** La Coordinación de Tutorías consta de 1 o 2 personas activas generando métricas. Renderizar documentos con QuestPDF compromete memoria RAM. Probar 3 descargas simultáneas verifica que el consumo se mantenga dentro del 1 GB de RAM sin `OutOfMemoryException`.<br>**Métrica:** Entrega en $< 3.5$ s con RAM $< 70\%$. |

---

## 4. Estructura de Archivos del Entorno de Pruebas
Los artefactos de prueba se organizan dentro del repositorio bajo la siguiente convención:

```text
tutorias/
├── openspec/
│   └── specs/
│       └── pruebas_no_funcionales_jmeter.md  <- Esta especificación
└── tests/
    └── jmeter/
        ├── datasets/                        <- Archivos .csv con datos sintéticos (alumnos, contraseñas)
        ├── plans/                           <- Planes .jmx diseñados en Apache JMeter
        └── results/                         <- Archivos .jtl y reportes dashboard HTML (en .gitignore)
```

---

## 5. Buenas Prácticas para la Ejecución con Apache JMeter

1. **Diseñar con GUI, Ejecutar por CLI (Headless):**
   Para no consumir recursos locales ni falsear mediciones, la interfaz gráfica de JMeter solo debe usarse para armar el plan `.jmx`. La ejecución real se hace por terminal:
   ```bash
   jmeter -n \
     -t tests/jmeter/plans/JMX-02_Catalogo.jmx \
     -l tests/jmeter/results/resultado_catalogo.jtl \
     -e -o tests/jmeter/results/dashboard_catalogo/
   ```

2. **Manejo del Token Antifalsificación en JMeter:**
   En peticiones `POST` protegidas con `[ValidateAntiForgeryToken]`:
   - Añadir un *HTTP Cookie Manager*.
   - Ejecutar primero un muestreador `GET` a la vista del formulario.
   - Usar un *Regular Expression Extractor*:
     - **Nombre de referencia:** `tokenAntiforgery`
     - **Expresión Regular:** `name="__RequestVerificationToken" type="hidden" value="(.+?)"`
     - **Template:** `$1$`
   - Enviar `${tokenAntiforgery}` en el parámetro del `POST`.

3. **Monitoreo Simultáneo en Azure Portal:**
   Durante la prueba en Azure, vigilar las gráficas de:
   - **App Service:** *CPU Percentage* y *Memory Percentage*.
   - **SQL Database:** *DTU Percentage* y *Deadlocks*.
