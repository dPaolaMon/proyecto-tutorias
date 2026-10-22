# Plan de Pruebas Automatizadas con Selenium WebDriver

**Proyecto:** Sistema Web de Gestión y Visibilidad de Tutorías de UPIICSA  
**Tipo de Pruebas:** Pruebas Funcionales, de Interfaz de Usuario (UI) y de Extremo a Extremo (E2E)  
**Herramientas Principales:** C# .NET 8, xUnit, Selenium WebDriver y Selenium Manager  
**Patrón de Diseño:** Page Object Model (POM)  
**Fecha de Elaboración:** 21 de octubre de 2026  

---

## 1. Justificación y Alcance

A diferencia de las pruebas no funcionales realizadas con Apache JMeter (enfocadas en la latencia de red y concurrencia a nivel de protocolo HTTP), las pruebas automatizadas con Selenium WebDriver interactúan directamente con el navegador web (Google Chrome / Firefox). 

Su propósito es certificar:
1. El funcionamiento continuo de los flujos de negocio transversales (E2E) entre los roles de Alumno, Tutor y Coordinador.
2. El correcto comportamiento del motor de JavaScript en el cliente (modales interactivos de Bootstrap 5, temporizadores regresivos en tiempo real y componentes asíncronos).
3. El cumplimiento del enfoque de diseño responsivo (*Mobile-First*), verificando la adaptabilidad en pantallas móviles (375x667) y de escritorio (1920x1080).
4. La aplicación efectiva de salvaguardas de seguridad en la interfaz gráfica (ocultamiento de componentes restringidos y bloqueo visual ante accesos indebidos).

---

## 2. Marco Tecnológico y Arquitectura de Pruebas

- **Framework de Ejecución:** Proyecto de pruebas integrado en la solución de .NET 8 con **xUnit** y **FluentAssertions**, permitiendo su ejecución local mediante el comando estandarizado `dotnet test`.
- **Automatización del Navegador:** **Selenium.WebDriver** (versión 4.x) con soporte de *Selenium Manager*, eliminando la necesidad de gestionar manualmente los binarios de `chromedriver`.
- **Patrón Page Object Model (POM):**
  - **Capa de Páginas (`PageObjects/`):** Centraliza los selectores del DOM (IDs, selectores CSS, XPath) y las acciones atómicas (ingresar texto, hacer clic, leer alertas).
  - **Capa de Pruebas (`Specs/`):** Contiene la lógica del flujo de negocio y las aserciones de validación, abstrayéndose de la estructura del HTML.
- **Configuración de Ejecución:** Soporta modo con interfaz visible (*Headed*) para depuración en desarrollo y modo sin cabeza (*Headless*) para entornos de integración continua o servidores Linux sin servidor gráfico.

---

## 3. Matriz de Casos de Prueba Seleccionados

| Identificador | Clasificación | Módulos Involucrados | Requerimientos Asociados | Valor y Aporte al Sistema |
| :--- | :--- | :--- | :--- | :--- |
| **SEL-01** | Extremo a Extremo (E2E) | Autenticación, Solicitudes, Perfil, Catálogo y Asistencia QR | RF-01, RF-06, RF-07, RF-09, RF-12, RF-18, RF-19 | **Crítico:** Valida el flujo troncal del sistema a través de los tres roles (Alumno postula, Coordinador aprueba, Tutor publica sesión y Alumno valida asistencia por QR). |
| **SEL-02** | Responsividad y UI | Catálogo Público y Menú Institucional | RF-13 | **Alto:** Certifica que el menú colapsable (Navbar Toggler) y las tarjetas de oferta académica se rendericen sin desbordamiento en viewport móvil (375x667). |
| **SEL-03** | Validación de Formularios | Registro Institucional y Perfil | RF-01, RF-10 | **Medio-Alto:** Verifica el rechazo de correos no pertenecientes al IPN (`@gmail.com`) y el bloqueo de caracteres no numéricos en el campo de boleta. |
| **SEL-04** | Seguridad y Roles (RBAC) | Coordinación y Panel de Usuario | RF-03, RF-05 | **Alto:** Valida que un alumno no visualice opciones administrativas y que la navegación forzada por URL a `/Coordinacion` redirija a la vista de acceso denegado. |

---

## 4. Especificación Detallada de los Casos de Prueba

### Caso SEL-01: Ciclo Completo de Tutoría y Asistencia QR (E2E)
Este flujo valida la interoperabilidad completa del ciclo académico:

1. **Fase A (Postulación de Alumno):**
   - El alumno inicia sesión en `/Account/Login`.
   - Navega al formulario de postulación a tutor y envía la solicitud para la modalidad *Tutoría entre Pares*.
   - Aserción: Visualización de alerta de éxito confirmando el estado `Pendiente`.
   - El alumno cierra sesión.
2. **Fase B (Dictamen en Coordinación):**
   - El Coordinador inicia sesión con cuenta oficial (`coordinacion@ipn.mx`).
   - Accede a `/Coordinacion/SolicitudesTutores` y ubica la solicitud del alumno.
   - Acciona el modal de aprobación y confirma la promoción del usuario.
   - Aserción: La solicitud cambia visualmente al estado `Aprobada`.
   - El Coordinador cierra sesión.
3. **Fase C (Configuración del Perfil de Tutor):**
   - El alumno postulado vuelve a iniciar sesión; el sistema le presenta la interfaz extendida de `Tutor`.
   - Ingresa su semblanza académica y registra un horario de atención con salón asignado.
   - Aserción: El estatus del perfil indica publicado (`EsPublico = true`).
4. **Fase D (Generación y Validación de Asistencia QR):**
   - El tutor genera una sesión activa en `/Asistencia/GenerarQr`, proyectando el código QR con su temporizador regresivo de 20 minutos.
   - Selenium captura el token dinámico de la sesión desde el atributo de datos en el DOM.
   - Un segundo alumno inicia sesión, accede a la interfaz de validación (`/Asistencia/Escanear`), envía el token capturado y confirma su registro.
   - Aserción: Se despliega la pantalla de confirmación exitosa con la materia, el tutor asignado y la fecha/hora oficial.

---

### Caso SEL-02: Verificación de Principio Mobile-First en el Catálogo
- **Configuración:** Redimensionamiento del navegador a 375x667 píxeles (dispositivo móvil estándar).
- **Acciones:**
  1. Navegar a `/Catalogo/Index`.
  2. Verificar que la barra de navegación institucional esté colapsada y el botón de menú hamburguesa sea visible y funcional.
  3. Desplegar el menú y validar la interactividad de los enlaces.
  4. Comprobar que las tarjetas de tutores se organicen en una sola columna vertical sin generar barras de desplazamiento horizontal indeseadas.
  5. Interactuar con el acordeón de filtros de búsqueda y comprobar su apertura fluida.

---

### Caso SEL-03: Validación de Reglas Institucionales de Captura
- **Acciones:**
  1. Acceder al formulario de registro en `/Account/Register`.
  2. Intentar enviar el formulario con un correo comercial externo (ejemplo: `usuario@gmail.com`).
  3. Aserción: Verificación del mensaje de error institucional en color rojo indicando que el dominio debe ser `@ipn.mx` o `@alumno.ipn.mx`.
  4. Intentar capturar caracteres alfabéticos en el campo "Boleta" (ejemplo: `2026A12345`).
  5. Aserción: Verificación del bloqueo y advertencia de formato numérico exclusivo.

---

### Caso SEL-04: Control de Acceso por Roles (RBAC UI) y Salvaguardas
- **Acciones:**
  1. Iniciar sesión con una cuenta de rol `Alumno`.
  2. Verificar en la barra de navegación la ausencia total del enlace "Coordinación" y del menú de gestión institucional.
  3. Intentar ingresar manualmente a la URL `/Coordinacion/Index`.
  4. Aserción: El navegador debe interceptar la petición y redirigir inmediatamente a `/Account/AccessDenied` o `/Account/Login`.

---

## 5. Estructura de Directorios Propuesta en el Repositorio

La implementación en código se estructurará de la siguiente manera:

```text
tutorias/
├── docs/
│   └── plan_pruebas_selenium.md           <- Este documento de planeación
└── tests/
    └── Selenium/
        ├── Drivers/
        │   └── WebDriverFactory.cs        <- Factoria de Chrome/Firefox (Normal / Headless)
        ├── PageObjects/
        │   ├── BasePage.cs                <- Esperas explicitas y metodos comunes
        │   ├── LoginPage.cs               <- Selectores y acciones de autenticacion
        │   ├── CatalogoPage.cs            <- Filtros y tarjetas de tutores
        │   ├── TutorPerfilPage.cs         <- Captura de horarios y semblanza
        │   ├── CoordinacionPage.cs        <- Bandeja de solicitudes y modales
        │   └── AsistenciaQrPage.cs        <- Proyeccion de QR y captura de asistencia
        └── Specs/
            ├── SEL01_E2E_CicloTutoriaTests.cs
            ├── SEL02_CatalogoResponsivoTests.cs
            ├── SEL03_ValidacionRegistroTests.cs
            └── SEL04_ControlAccesoRbacTests.cs
```
