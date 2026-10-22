# Informe Técnico: Implementación, Calibración y Estabilización de Pruebas Automatizadas con Selenium WebDriver

**Proyecto:** Sistema Web de Gestión y Visibilidad de Tutorías de UPIICSA  
**Módulo Evaluado:** Suite de Pruebas Funcionales, de Interfaz (UI) y Extremo a Extremo (E2E)  
**Herramienta Utilizada:** Selenium WebDriver 4.x con C# / .NET 8 y xUnit  
**Navegador de Automatización:** Mozilla Firefox ESR (Modo Headed / Headless)  
**Fecha:** 22 de octubre de 2026  
**Destinatario:** Coordinación Técnica y Supervisión de Proyecto  

---

## 1. Resumen Ejecutivo

El presente informe documenta el proceso técnico de construcción, ejecución inicial y estabilización (*Test Hardening*) de la suite de pruebas automatizadas con Selenium WebDriver para el Sistema de Tutorías de UPIICSA.

Durante la fase inicial de ejecución de pruebas de navegador, es una constante metodológica documentada en la ingeniería de pruebas que surjan discrepancias de sincronización e interferencias de elementos en el DOM. Este informe describe las causas técnicas de dichas incidencias, las soluciones de ingeniería implementadas y el estado de madurez alcanzado por la suite de calidad.

---

## 2. Naturaleza del Proceso: Fase de Estabilización de Pruebas (Test Hardening)

En el ciclo de vida del aseguramiento de calidad (QA), las pruebas de extremo a extremo (E2E) representan el nivel de mayor exigencia y acoplamiento, dado que evalúan simultáneamente:
1. El motor de JavaScript y el ciclo de vida de renderizado CSS del navegador real.
2. La velocidad de ejecución del código de prueba frente a la latencia asíncrona del navegador.
3. El servidor web de aplicaciones (.NET 8 Kestrel).
4. El motor de base de datos relacional (Microsoft SQL Server).

La aparición de excepciones por tiempo de espera (*Timeouts*) en las primeras corridas es parte integral y natural del proceso. Su resolución sistemática permite pasar de un conjunto de pruebas tentativo a una suite automatizada robusta, determinista y confiable.

---

## 3. Incidencias Identificadas y Acciones Correctivas

A continuación se detallan las incidencias técnicas registradas durante las primeras ejecuciones y las soluciones aplicadas a nivel arquitectónico:

### 3.1 Incidencia 1: Ambigüedad en Selectores del DOM y Elementos Ocultos
- **Síntoma:** Excepción `WebDriverTimeoutException` en la acción de filtrado del catálogo durante la prueba E2E (`SEL-01`).
- **Análisis Causa-Raíz:**  
  En la barra de navegación institucional superior (`_Layout.cshtml`), existe un formulario para cerrar sesión que contiene un botón `<button type="submit">`. Dicho botón reside dentro de un menú desplegable de usuario (`#userDropdown`) que permanece oculto (`display: none`) mientras no se le haga clic.  
  El selector inicial en `CatalogoPage.cs` utilizaba una regla genérica (`form button[type='submit']`), provocando que Selenium localizara en primer lugar el botón de cerrar sesión. Al estar oculto en pantalla, el evaluador de Selenium esperó de forma infructuosa durante 10 segundos a que el elemento se hiciera visible.
- **Acción Correctiva:**  
  1. Se refactorizó el selector en `CatalogoPage.cs` para delimitarlo estrictamente al formulario de búsqueda del catálogo (`form[method='get'] button[type='submit']`).
  2. En la clase base de automatización (`BasePage.cs`), se sustituyó la búsqueda directa por un algoritmo de inspección sobre colecciones que filtra y selecciona de manera automática únicamente aquellos elementos que estén simultáneamente visibles y habilitados (`Displayed && Enabled`).

---

### 3.2 Incidencia 2: Carrera de Tiempo (Race Condition) por Animaciones CSS de Bootstrap 5
- **Síntoma:** Falla de aserción en la prueba de responsividad móvil (`SEL-02`), indicando que el menú de navegación no se encontraba desplegado tras accionar el botón de hamburguesa.
- **Análisis Causa-Raíz:**  
  El componente colapsable de Bootstrap 5 implementa una transición animada de altura que dura aproximadamente 350 milisegundos. Durante esta ventana de animación, el elemento transita por la clase intermedia `collapsing` antes de consolidar la clase definitiva `collapse navbar-collapse show`.  
  El código de prueba realizaba una lectura síncrona en microsegundos inmediatamente después del clic, evaluando el estado antes de que concluyera la transición gráfica.
- **Acción Correctiva:**  
  Se implementó una espera explícita condicionada mediante `WebDriverWait` en el método `EsMenuNavegacionDesplegado()`. De este modo, Selenium sincroniza su ejecución con el motor de renderizado del navegador, confirmando el estado una vez que la animación CSS concluye exitosamente.

---

### 3.3 Incidencia 3: Desacoplamiento de Dependencias de Configuración
- **Síntoma:** Advertencias de compilación por falta de extensiones `GetValue<T>` y `AddEnvironmentVariables`.
- **Análisis Causa-Raíz:**  
  Dependencia de paquetes NuGet opcionales para lectura de variables de entorno y mapeo de configuraciones.
- **Acción Correctiva:**  
  Se reescribió `WebDriverFactory.cs` para operar directamente con la biblioteca base de .NET 8 (`System.Text.Json` y `Environment.GetEnvironmentVariable`), garantizando compilación sin advertencias ni sobrecarga de paquetes externos.

---

### 3.4 Incidencia 4: Bloqueo de Conexión en Navegación Inicial (HttpClient Timeout de 60 segundos por PageLoadStrategy)
- **Síntoma:** Excepción `WebDriverException` con mensaje `The HTTP request to the remote WebDriver server timed out after 60 seconds` durante la invocación de `LoginPage.IrALogin()`.
- **Análisis Causa-Raíz:**  
  Por defecto, Selenium WebDriver opera bajo la estrategia de carga `PageLoadStrategy.Normal`. Bajo este esquema, la instrucción `Navigate().GoToUrl()` retiene el socket HTTP del controlador del navegador hasta que el evento `window.onload` se dispara en su totalidad, aguardando la descarga completa de cualquier recurso periférico (hojas de estilo remotas, tipografías externas o scripts asíncronos). Si el servidor local Kestrel experimenta latencia por arranque en frío (*Cold Start*) o si el navegador espera un subrecurso, el canal de comunicación entre el proceso de prueba y el servicio WebDriver agota su tiempo límite de 60 segundos y cancela el socket (`TaskCanceledException`).
- **Acción Correctiva:**  
  1. Se configuró explícitamente `PageLoadStrategy = PageLoadStrategy.Eager` en `WebDriverFactory.cs` tanto para Firefox como para Chrome. Esta política permite que Selenium retome el control de ejecución en cuanto el DOM principal alcanza el estado interactivo (`DOMContentLoaded`), sin esperar a que finalice la descarga de componentes periféricos no esenciales.
  2. Se acotó el límite de espera de navegación a nivel de driver con `driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30)`.
  3. Se estableció como protocolo operativo verificar que el servidor local (`http://localhost:5178`) esté activo y respondiendo previamente al despacho de la suite.

---

## 4. Estado Actual de la Suite de Calidad

Tras la aplicación de las correcciones, la suite de pruebas automatizadas presenta la siguiente cobertura y estado de operatividad:

| Identificador | Caso de Prueba | Enfoque Técnico | Estado |
| :--- | :--- | :--- | :--- |
| **SEL-01** | Ciclo E2E de Tutoría y Catálogo | Autenticación de Coordinador, validación de tabla de gestión de usuarios, acceso a solicitudes de tutores y filtrado de catálogo. | **Operativo / Exitoso** |
| **SEL-02** | Verificación Mobile-First | Redimensionamiento a resolución móvil (375x667), comprobación de menú hamburguesa y renderizado responsivo. | **Operativo / Exitoso** |
| **SEL-03** | Validación de Dominio Institucional | Intento de registro con correo comercial externo y validación de rechazo estricto en la interfaz. | **Operativo / Exitoso** |
| **SEL-04** | Seguridad y Control de Acceso (RBAC) | Intento de navegación no autorizada a rutas administrativas y redirección obligatoria a inicio de sesión. | **Operativo / Exitoso** |

---

## 5. Conclusiones y Valor para el Proyecto

1. **Robustez Garantizada:** La suite de pruebas con Selenium ha superado la fase de estabilización, transformándose en una herramienta repetible y determinista.
2. **Prevención de Regresiones:** El equipo cuenta ahora con una barrera de contención automática que permite validar en segundos que los futuros cambios de código no rompan la experiencia de usuario, los privilegios de acceso ni la responsividad del portal.
3. **Compatibilidad Local y Nube:** La infraestructura de pruebas soporta tanto la ejecución en servidores locales de desarrollo como la validación directa contra el despliegue en Microsoft Azure.
