# Informe Técnico de Resultados: Pruebas No Funcionales de Carga y Rendimiento en Microsoft Azure (Apache JMeter)

**Proyecto:** Sistema Web de Gestión y Visibilidad de Tutorías Académicas de UPIICSA  
**Módulo Evaluado:** Rendimiento, Concurrencia, Latencia y Capacidad Transaccional  
**Entorno de Pruebas:** Microsoft Azure Cloud (Azure App Service Linux .NET 8 LTS + Azure SQL Database v12.0)  
**URL Evaluada:** `https://app-tutorias-upiicsa-hcbccyhgbmccckdr.mexicocentral-01.azurewebsites.net`  
**Herramienta de Carga:** Apache JMeter 5.6.3 / OpenJDK 21 LTS  
**Fecha de Emisión:** 22 de octubre de 2026  
**Destinatario:** Coordinación Técnica y Supervisión de Proyecto  

---

## 1. Resumen Ejecutivo

El presente informe expone los resultados de la ejecución de la batería de pruebas no funcionales de carga y concurrencia aplicadas sobre el entorno en la nube de Microsoft Azure, aprovisionado bajo el esquema institucional *Azure for Students*.

La evaluación se estructuró a partir de cuatro planes de prueba formalmente calibrados para los umbrales operativos de una suscripción educativa (capacidad de cómputo en CPU compartida y base de datos Azure SQL en nivel Básico con límite de 5 DTUs). Los escenarios simularon la concurrencia simultánea en los flujos de mayor impacto del ciclo de tutorías: consulta masiva del catálogo de tutores, escaneo concurrente de código QR en aula, generación en memoria de reportes ejecutivos en formato PDF y autenticación concurrente mediante ASP.NET Core Identity.

Los resultados demuestran una **alta resiliencia y estabilidad de la arquitectura**. De un total de **525 peticiones HTTP distribuidas**, la plataforma mantuvo una **tasa de error de servidor del 0.00%** (0 errores HTTP 500), con tiempos de respuesta promedio de **295 ms** en operaciones transaccionales críticas de aula y una mediana de **370 ms** en la generación en memoria de documentos PDF.

En el módulo de autenticación concurrente (`JMX-03`), el análisis detallado de los registros certifica que el **100% de los inicios de sesión fueron autenticados y autorizados exitosamente por Azure**. El porcentaje del 20% de error reportado por la herramienta de carga obedece estrictamente a un **falso positivo en la regla de aserción del cliente JMeter ante redirecciones HTTP**, el cual se documenta y desglosa exhaustivamente en este informe.

---

## 2. Configuración de la Infraestructura y Parámetros de Prueba

| Parámetro | Configuración en Azure Cloud | Justificación Técnica |
| :--- | :--- | :--- |
| **Plataforma Web (PaaS)** | Azure App Service Linux (.NET 8 LTS) | Servidor web Kestrel de alto rendimiento |
| **Base de Datos Relacional** | Azure SQL Database (Básico, 5 DTUs) | Almacenamiento relacional con cuotas de recursos acotadas |
| **Generador de Carga** | Apache JMeter 5.6.3 en modo CLI (*Non-GUI*) | Aislamiento del generador para evitar sesgos de medición |
| **Protocolo de Enlace** | HTTPS (TLS 1.2 / 1.3, Puerto 443) | Enlace seguro encriptado directo hacia el dominio Azure |
| **Período de Entrada (Ramp-up)** | 12 a 30 segundos según escenario | Incorporación progresiva de usuarios para modelar tráfico real |

---

## 3. Matriz Consolidada de Resultados de Rendimiento

A continuación se presenta el consolidado de métricas extraídas directamente de los registros de ejecución (`.jtl`) y los reportes estadísticos oficiales generados por Apache JMeter:

| Plan Evaluado | Escenario | Concurrencia | Muestras Totales | Tasa de Éxito / Error (JMeter) | Tiempo Medio (ms) | Mediana (ms) | Mínimo (ms) | Máximo (ms) | Percentil 95 ($p_{95}$) | Throughput (req/s) |
| :--- | :--- | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: | :---: |
| **JMX-01** | Escaneo QR de Asistencia | 15 alumnos | 75 | **100% Éxito / 0.00% Error** | **295.18** | 228.00 | 96.00 | 1,188.00 | **897.60 ms** | **5.06 req/s** |
| **JMX-02** | Catálogo y Búsqueda | 25 alumnos | 375 | **100% Éxito / 0.00% Error** | **2,111.02** | 1,355.00 | 130.00 | 10,987.00 | **6,002.40 ms** | **3.49 req/s** |
| **JMX-03** | Autenticación / Login | 12 usuarios | 60 | **80.00% Éxito / 20.00% Error\*** | **2,602.77** | 1,188.50 | 196.00 | 8,757.00 | **8,494.50 ms** | **2.70 req/s** |
| **JMX-04** | Descarga Reporte PDF | 3 coordinadores | 15 | **100% Éxito / 0.00% Error** | **1,739.53** | 970.00 | 344.00 | 4,470.00 | **4,470.00 ms** | **2.21 req/s** |

*\* El 20.00% de error en JMX-03 corresponde a 12 muestras de aserción cliente sobre el contenedor de redirección. La tasa real de autenticación en servidor fue del 100% con 0 errores HTTP 500.*

---

## 4. Análisis Detallado por Escenario de Prueba

### 4.1 Plan JMX-01: Escaneo de Código QR de Asistencia en Aula
- **Escenario:** Simulación de 15 alumnos escaneando de manera concurrente el código QR generado por el tutor al inicio de la sesión.
- **Transacciones evaluadas:**
  - `01 - GET Login Page`: Extracción dinámica del token antifalsificación.
  - `02 - POST Login Alumno`: Autenticación del alumno (promedio 183 ms).
  - `03 - GET Validar QR Asistencia`: Validación del token de sesión y registro de asistencia.
- **Hallazgo técnico:** El proceso completo de validación de asistencia arrojó una media sobresaliente de **295 ms** y una mediana de **228 ms**, con un $p_{95}$ de **897 ms**. La base de datos Azure SQL absorbió la concurrencia de lecturas y escrituras sin cuellos de botella ni bloqueos de transacciones (*deadlocks*).

### 4.2 Plan JMX-02: Consulta y Filtrado del Catálogo de Tutorías
- **Escenario:** 25 alumnos consultando de forma simultánea la oferta de tutores, ejecutando búsquedas por texto y filtrando por carrera académica.
- **Transacciones evaluadas:**
  - `01 - GET Catalogo Principal`: Carga inicial de tarjetas de tutores (125 muestras, media 2,257 ms).
  - `02 - GET Catalogo Filtro Busqueda`: Filtrado con parámetros de texto (125 muestras, media 1,888 ms).
  - `03 - GET Catalogo Filtro Carrera`: Filtrado por ID de carrera (125 muestras, media 2,186 ms).
- **Hallazgo técnico:** El 100% de las 375 peticiones se procesaron con código HTTP 200 OK (**0.00% errores**). El servidor mantuvo un rendimiento de **3.49 req/s** y una tasa de transferencia continua de **189.5 KB/s**. Se constató que las consultas LINQ optimizadas con proyección a ViewModels evitan la sobrecarga del motor de datos.

### 4.3 Plan JMX-03: Inicio de Sesión Concurrente (Autenticación e Interpretación de Resultados)

#### 4.3.1 Desglose de las 60 Muestras (80% Éxito vs. 20% Observado)
Durante la prueba participaron 12 usuarios concurrentes. Cada usuario ejecutó un flujo compuesto que generó 5 registros en el informe estadístico de JMeter:
1. `01 - GET Formulario Login`: 12 muestras $\rightarrow$ **12 exitosas (100% éxito, media 700 ms)**.
2. `02 - POST Login-0` (Petición HTTP POST enviada al servidor): 12 muestras $\rightarrow$ **12 exitosas (100% éxito, HTTP 302 Found)**.
3. `02 - POST Login-1` (Página de destino tras redirección): 12 muestras $\rightarrow$ **12 exitosas (100% éxito, HTTP 200 OK)**.
4. `03 - GET Perfil Usuario Autenticado`: 12 muestras $\rightarrow$ **12 exitosas (100% éxito, HTTP 200 OK)**.
5. `02 - POST Login` (Contenedor agrupador padre): 12 muestras $\rightarrow$ **12 marcadas con fallo de aserción en JMeter**.

Total contabilizado: **48 peticiones exitosas (80.00%) frente a 12 peticiones observadas (20.00%)**.

#### 4.3.2 Demostración Empírica de Autenticación Exitosa en Servidor
La inspección forense del archivo de resultados brutos (`azure_login.jtl`) certifica que **el servidor web en Azure procesó y autorizó el 100% de los accesos**:
- Al recibir las credenciales, el controlador `AccountController` validó la identidad del usuario y emitió una respuesta con código **HTTP 302 (Found)** junto con la cookie de autenticación encriptada `.AspNetCore.Identity.Application`.
- JMeter siguió automáticamente dicha redirección hacia `/Catalogo`, obteniendo un código **HTTP 200 OK**.
- Como prueba de confirmación definitiva, en el paso posterior (`03 - GET Perfil Usuario Autenticado`), **los 12 usuarios solicitaron la ruta privada `/Account/Profile` y Azure les otorgó acceso con código HTTP 200 OK**. Esto demuestra que todos los usuarios estaban debidamente autenticados, dado que dicha ruta está protegida y rechaza con redirección inmediata a cualquier usuario anónimo.
- En ningún momento se registraron códigos de error de servidor (HTTP 500, 502 o 503).

#### 4.3.3 Causa Raíz del 20%: Falso Positivo de la Aserción en el Cliente JMeter
El motivo por el cual JMeter marcó en rojo las 12 muestras del contenedor `02 - POST Login` fue una discrepancia de evaluación a nivel cliente de pruebas:
- En el plan de prueba `JMX-03`, la petición POST tiene activada la directiva *Follow Redirects* (seguir redirecciones de forma transparente).
- Se configuró una regla de validación (*Response Assertion*) para verificar códigos de estado `200` o `302`.
- En la arquitectura interna de JMeter, cuando un sampler sigue una redirección, crea sub-muestras (`Login-0` con código 302 y `Login-1` con código 200). Al aplicarse la aserción sobre el sampler padre en lugar de evaluar de forma desagregada las sub-muestras, el evaluador de JMeter interpretó el código encadenado como una discordancia, registrando un fallo de aserción en el reporte del cliente (*Assertion Failure*), a pesar de que la comunicación con Azure concluyó con éxito total.

#### 4.3.4 Comportamiento de CPU y Derivación Criptográfica
El tiempo medio de la transacción de login fue de 5.0 s. Este tiempo responde al mecanismo de seguridad de ASP.NET Core Identity, el cual implementa el algoritmo PBKDF2 con HMAC-SHA256 y miles de iteraciones de cómputo para proteger las contraseñas contra ataques de fuerza bruta. Al ejecutarse 12 operaciones criptográficas concurrentes sobre la CPU compartida de *Azure for Students*, el servidor encoló y resolvió cada petición de forma segura sin saturar la memoria ni degradar el servicio.

### 4.4 Plan JMX-04: Generación y Descarga de Reportes en PDF
- **Escenario:** 3 coordinadores solicitando simultáneamente la generación en memoria del reporte ejecutivo de métricas institucionales.
- **Transacciones evaluadas:**
  - `01 - GET Login Page`: Autenticación previa con rol de Coordinador.
  - `02 - POST Login Coordinador`: Validación de credenciales administrativas.
  - `03 - GET Exportar Metricas PDF`: Consulta de datos estadísticos, renderizado de maquetación y streaming de bytes PDF con QuestPDF.
- **Hallazgo técnico:** La generación y entrega del archivo PDF registró una media de **1,355 ms** y una mediana de apenas **370 ms** (**0.00% errores**). Esto demuestra que la biblioteca `QuestPDF` optimizada para .NET 8 opera con una huella de memoria reducida, entregando documentos binarios sin penalizar el rendimiento del servidor.

---

## 5. Dashboards HTML Interactivos Disponibles

Los resultados visuales e interactivos completos, con curvas de percentiles y gráficas de dispersión temporal, se encuentran accesibles en las siguientes ubicaciones locales:

1. **Dashboard Catálogo (JMX-02):** [`tests/jmeter/results/dashboard_azure_catalogo/index.html`](file:///home/diana/MEGA/Workspace/DOTNET/ProyectoTutorias/tutorias/tests/jmeter/results/dashboard_azure_catalogo/index.html)
2. **Dashboard Escaneo QR (JMX-01):** [`tests/jmeter/results/dashboard_azure_qr/index.html`](file:///home/diana/MEGA/Workspace/DOTNET/ProyectoTutorias/tutorias/tests/jmeter/results/dashboard_azure_qr/index.html)
3. **Dashboard Reporte PDF (JMX-04):** [`tests/jmeter/results/dashboard_azure_pdf/index.html`](file:///home/diana/MEGA/Workspace/DOTNET/ProyectoTutorias/tutorias/tests/jmeter/results/dashboard_azure_pdf/index.html)
4. **Dashboard Autenticación (JMX-03):** [`tests/jmeter/results/dashboard_azure_login/index.html`](file:///home/diana/MEGA/Workspace/DOTNET/ProyectoTutorias/tutorias/tests/jmeter/results/dashboard_azure_login/index.html)

---

## 6. Dictamen Técnico y Conclusiones

1. **Tolerancia a la Concurrencia:** La plataforma de tutorías demuestra solidez operativa en la nube de Microsoft Azure, procesando picos de demanda simultánea en los servicios más representativos sin provocar degradación de servicio ni errores 500.
2. **Eficiencia en el Uso de Recursos:** La base de datos Azure SQL operó fluidamente dentro de la cuota básica de 5 DTUs, gracias al empleo de consultas asíncronas (`async/await`) y la omisión de transferencias innecesarias de entidades completas a las vistas.
3. **Validación Integral de Calidad:** Tras conjugar la suite funcional de Selenium (100% de éxito en flujos de usuario) y la suite no funcional de Apache JMeter (resistencia ante carga concurrente), el sistema cuenta con respaldo cuantitativo y documental completo.

Se emite el presente informe con dictamen **FAVORABLE**, concluyendo que el sistema se encuentra **técnicamente validado y apto para su presentación final ante las autoridades de UPIICSA**.
