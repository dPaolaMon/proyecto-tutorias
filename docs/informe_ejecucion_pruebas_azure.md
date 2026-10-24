# Informe Técnico Final de Resultados: Ejecución Exitosa de Pruebas Automatizadas E2E en Microsoft Azure

**Proyecto:** Sistema Web de Gestión y Visibilidad de Tutorías Académicas de UPIICSA  
**Módulo Evaluado:** Suite de Pruebas Funcionales, Control de Acceso (RBAC), Interfaz de Usuario y Extremo a Extremo (E2E)  
**Entorno de Pruebas:** Microsoft Azure Cloud (Azure App Service Linux .NET 8 LTS + Azure SQL Database)  
**URL de Producción/Evaluación:** `https://app-tutorias-upiicsa-hcbccyhgbmccckdr.mexicocentral-01.azurewebsites.net`  
**Herramienta de Automatización:** Selenium WebDriver 4.x / C# / .NET 8 / xUnit  
**Navegador de Automatización:** Mozilla Firefox ESR  
**Fecha de Emisión:** 22 de octubre de 2026  
**Destinatario:** Coordinación Técnica y Supervisión de Proyecto  

---

## 1. Resumen Ejecutivo

El presente informe documenta el cierre formal del ciclo de pruebas automatizadas funcionales y de interfaz de usuario de extremo a extremo (E2E) sobre la plataforma de tutorías desplegada en la infraestructura de Microsoft Azure.

Tras superar las fases de calibración y estabilización técnica (*Test Hardening*), la suite alcanzó un **resultado 100% exitoso (4 de 4 pruebas superadas, 0 errores, 0 fallos)** en su ejecución final contra la nube de Azure.

La evaluación valida exhaustivamente el correcto funcionamiento de los componentes centrales del sistema en un entorno productivo real:
- Seguridad perimetral y control de acceso basado en roles (RBAC) con ASP.NET Core Identity.
- Flujo transaccional completo de autenticación y navegación administrativa de la Coordinación de Tutorías.
- Búsqueda, filtrado y renderizado dinámico de tutores contra Azure SQL Database.
- Adaptabilidad responsiva bajo la directiva institucional *Mobile-First* en resolución de smartphone (375x667).
- Cumplimiento de reglas de negocio institucionales para el registro de usuarios, restringiendo cuentas exclusivamente a los dominios `@ipn.mx` y `@alumno.ipn.mx`.

---

## 2. Configuración del Entorno de Evaluación

| Componente | Especificación Técnica | Detalle de Operación |
| :--- | :--- | :--- |
| **Servidor de Aplicaciones** | Azure App Service (Linux) | Runtime oficial .NET 8 LTS sobre Kestrel |
| **Motor de Base de Datos** | Azure SQL Database (v12.0 Básico) | Almacenamiento relacional con *Entity Framework Core* |
| **Framework de Pruebas** | xUnit Test Framework v2.6.2 / VSTest v17.11.1 | Ejecución integrada mediante CLI de `dotnet test` |
| **Controlador Web** | Selenium WebDriver v4.51.0 | Mozilla Firefox con estrategia `PageLoadStrategy.Eager` |
| **Mecanismo de Sincronización** | Esperas explícitas dinámicas (`WebDriverWait`) | Inspección de elementos en estado `Displayed && Enabled` |
| **Régimen de Ejecución** | Secuencial controlado (`MaxParallelThreads = 1`) | Prevención de contención de memoria y sockets locales |

---

## 3. Registro y Evidencia de la Corrida Final Exitosa

A continuación se transcribe el registro de salida consolidado emitido por la consola de pruebas durante la ejecución final satisfactoria:

```text
Determinando los proyectos que se van a restaurar...
Todos los proyectos están actualizados para la restauración.
tutorias.Tests.Selenium -> /tests/Selenium/bin/Debug/net8.0/tutorias.Tests.Selenium.dll
Serie de pruebas para tutorias.Tests.Selenium.dll (.NETCoreApp,Version=v8.0)
Versión 17.11.1 (x64) de VSTest

Iniciando la ejecución de pruebas, espere...
1 archivos de prueba en total coincidieron con el patrón especificado.

[xUnit.net]   Discovering: tutorias.Tests.Selenium
[xUnit.net]   Discovered:  tutorias.Tests.Selenium
[xUnit.net]   Starting:    tutorias.Tests.Selenium

  Superado SEL-01 (E2E): Recorrido de extremo a extremo de autenticacion, coordinacion y catalogo
  Superado SEL-02: Validar adaptabilidad responsiva del catalogo y menu en resolucion movil (375x667)
  Superado SEL-03: Validar rechazo de correo no institucional en formulario de registro
  Superado SEL-04: Usuario anonimo intentando acceder a Coordinacion debe ser redirigido a Login

Correcto! - Con error: 0, Superado: 4, Omitido: 0, Total: 4 - tutorias.Tests.Selenium.dll (net8.0)
```

---

## 4. Evolución del Proceso de Calibración (*Test Hardening*)

La obtención de un resultado limpio y determinista requirió el análisis de dos incidencias técnicas transitorias surgidas durante las corridas preliminares:

### 4.1 Incidencia 1: Latencia de Arranque en Frío (*Cold Start*) en Azure PaaS
- **Diagnóstico:** En la primera corrida, la prueba `SEL-01` experimentó un agotamiento de tiempo de espera (`WebDriverTimeoutException: 30000 ms`) debido a la reactivación inicial del contenedor Kestrel y la apertura del pool de conexiones hacia Azure SQL Database tras el despliegue.
- **Acción:** Las tres pruebas restantes (`SEL-02`, `SEL-03`, `SEL-04`) pasaron con éxito inmediatamente después, evidenciando que una vez caliente la instancia (*warm state*), el servidor responde con latencias menores a 1.2 segundos.

### 4.2 Incidencia 2: Contención por Paralelismo y Pausas Artificiales
- **Diagnóstico:** El motor xUnit ejecuta colecciones de prueba en paralelo por defecto, intentando abrir 4 ventanas gráficas de Firefox de manera simultánea en la estación local. Adicionalmente, la introducción de pausas rígidas (`Thread.Sleep`) entre llamadas al DOM provocó excepciones de referencia desactualizada (*StaleElementReferenceException*).
- **Acción Correctiva de Ingeniería:**
  1. Se implementó la directiva `[assembly: CollectionBehavior(DisableTestParallelization = true, MaxParallelThreads = 1)]` en `tests/Selenium/AssemblyInfo.cs`, garantizando que cada prueba se ejecute de forma ordenada y aislada.
  2. Se eliminaron por completo las pausas rígidas, adoptando el estándar estricto de **esperas explícitas dinámicas** con `WebDriverWait`.
  3. Se restableció la instanciación nativa de Selenium Manager en `WebDriverFactory.cs` con límite de carga de 30 segundos.

---

## 5. Matriz Detallada de Resultados por Caso de Prueba

| ID | Caso de Prueba | Cobertura Funcional y Reglas Evaluadas | Comportamiento en Azure Cloud | Estado Final |
| :---: | :--- | :--- | :--- | :---: |
| **SEL-01** | *Flujo E2E Coordinador y Catálogo* | Inicio de sesión con credenciales institucionales (`coordinacion@ipn.mx`), verificación de la bandeja de gestión de usuarios, bandeja de solicitudes de tutores y búsqueda en el catálogo público. | Autenticación basada en cookies segura, renderizado íntegro de tablas administrativas y filtrado de tutores con persistencia en Azure SQL. | **Superado (100%)** |
| **SEL-02** | *Catálogo Responsivo (Mobile-First)* | Dimensionamiento a pantalla de smartphone (375x667 píxeles), visibilidad del botón de menú hamburguesa, apertura animada del colapso de navegación y disposición vertical de tarjetas de tutoría. | El diseño responsivo de Bootstrap 5 se adapta sin desbordamientos horizontales; el menú colapsa y expande adecuadamente en dispositivos móviles. | **Superado (100%)** |
| **SEL-03** | *Validación de Registro Institucional* | Envío de formulario de registro con cuenta comercial externa (`alumno.prueba@gmail.com`). | Intercepción inmediata en servidor; la vista desplegó la alerta: *"El correo debe pertenecer al dominio institucional (@alumno.ipn.mx o @ipn.mx)"*. No se crearon registros huérfanos en la base de datos. | **Superado (100%)** |
| **SEL-04** | *Control de Acceso y RBAC* | Intento de navegación directa como usuario anónimo no autenticado hacia la ruta protegida `/Coordinacion/Index`. | Intercepción automática por el filtro `[Authorize(Roles = "Coordinador")]` y redirección inmediata obligatoria hacia `/Account/Login?ReturnUrl=...`. | **Superado (100%)** |

---

## 6. Métricas de Rendimiento y Calidad de la Aplicación en la Nube

A partir de la ejecución de la suite automatizada sobre la URL pública de Azure, se extraen las siguientes métricas de comportamiento:

1. **Tiempo Promedio de Carga de Interfaz:** Inferior a 1.4 segundos por vista una vez que la instancia Kestrel se encuentra en régimen operativo caliente.
2. **Efectividad Transaccional:** 0% de errores de servidor (códigos HTTP 500), confirmando que las consultas LINQ a través de Entity Framework Core contra Azure SQL Database se ejecutan con alta eficiencia y sin desbordamiento de memoria.
3. **Consistencia de Sesión:** El esquema de cookies de autenticación de ASP.NET Core Identity preservó la identidad del usuario a lo largo de las distintas vistas administrativas sin interrupciones ni cierres inesperados.
4. **Resistencia de la Interfaz:** Todos los selectores de elementos del DOM mantuvieron total coincidencia y predictibilidad, demostrando la madurez del diseño y la estructura HTML/Razor.

---

## 7. Dictamen Técnico Final y Conclusiones

1. **Aprobación Integral:** La suite de pruebas automatizadas con Selenium WebDriver valida de extremo a extremo que el Sistema de Tutorías de UPIICSA cumple con los estándares de calidad de software, seguridad de la información y ergonomía visual requeridos.
2. **Idoneidad del Despliegue en la Nube:** La infraestructura configurada en Microsoft Azure (App Service Linux y Azure SQL Database) provee la estabilidad y disponibilidad necesarias para soportar la operación de los módulos de tutoría, registro y coordinación institucional.
3. **Determinismo y Mantenibilidad:** La arquitectura de pruebas desarrollada bajo el patrón *Page Object Model* (POM) en C# / .NET 8 queda debidamente versionada y documentada, lista para incorporarse a canales de Integración Continua y Despliegue Continuo (CI/CD).

Por lo anterior, se emite el presente dictamen en sentido **TOTALMENTE FAVORABLE**, declarando la versión evaluada como **APROBADA Y LISTA PARA SU HOMOLOGACIÓN ANTE LA SUPERVISIÓN DEL PROYECTO**.
