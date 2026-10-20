# 📋Plan de Tareas Incrementales de Desarrollo

Este documento desglosa la implementación integral del **Sistema de Tutorías Académicas** en fases incrementales y verificables, basado en los 25 requerimientos funcionales oficiales (`RF.txt`), la arquitectura (`ARCHITECTURE.md`) y el modelo de datos (`data-model.md`).

---

## 🚀 Fase 1: Autenticación, Acceso y Gestión de Cuentas (RF-01 a RF-05)

### Tarea 1.1: Base de Datos, Catálogo UPIICSA y Modelado Base de Usuario (RF-01, RF-02, RF-03)
- [x] Crear entidad `ApplicationUser` extendiendo `IdentityUser` con `Nombre`, `ApellidoPaterno`, `ApellidoMaterno`, `Boleta` (solo dígitos), `CarreraId`, `Academia` y banderas de baja lógica (`Activo`, `FechaBaja`, `MotivoBaja`).
- [x] Crear entidades `Carrera`, `Materia` y `MateriaEquivalencia` con Seeder automático desde `upiicsa_catalogo.json`.
- [x] Implementar vistas responsivas de Login, Registro, Completar Perfil, Edición de Perfil y Baja Lógica.
- [x] Generar y aplicar migraciones iniciales de EF Core a la base de datos SQL Server / Azure SQL.

### Tarea 1.2: Verificación de Correo Institucional en Registro (RF-01)
- [x] Diseñar interfaz `IEmailSenderService` y registrar dos implementaciones: `DevEmailService` (desarrollo, logger en consola y enlace de depuración visual) y `AzureEmailService` (producción vía Azure Communication Services).
- [x] Configurar ASP.NET Core Identity con `options.SignIn.RequireConfirmedEmail = true`.
- [x] Modificar `AccountController.Register` para crear el usuario con `EmailConfirmed = false`, generar el token con `UserManager.GenerateEmailConfirmationTokenAsync` y enviar el enlace sin iniciar sesión directo.
- [x] Implementar vista `RegisterConfirmation.cshtml` con mensaje institucional y botón de depuración en modo local (`@if (appEnv.IsDevelopment())`).
- [x] Implementar acción `GET /Account/ConfirmEmail` que valide `userId` y `token`, asiente la confirmación con `ConfirmEmailAsync`, inicie sesión automáticamente y redirija a la Pantalla de Inicio (`/Home/Index`).
- [x] Bloquear inicio de sesión en `AccountController.Login` si `EmailConfirmed == false` y proveer flujo de reenvío con `ResendConfirmationEmail`.

### Tarea 1.3: Recuperación de Contraseña (RF-04)
- [x] Implementar `ForgotPasswordViewModel` solicitando correo institucional (`@alumno.ipn.mx` o `@ipn.mx`).
- [x] Crear acciones `ForgotPassword` y `ResetPassword` en `AccountController` usando `GeneratePasswordResetTokenAsync` y `ResetPasswordAsync`.
- [x] Diseñar vistas Razor `ForgotPassword.cshtml`, `ForgotPasswordConfirmation.cshtml` y `ResetPassword.cshtml`.
- [x] Verificar que no se revele la existencia de correos no registrados al solicitar restablecimiento.

### Tarea 1.4: Desactivación de Usuarios y Gestión de Roles en Coordinación (RF-05)
- [x] Crear `CoordinacionController` protegido con `[Authorize(Roles = "Coordinador")]`.
- [x] Crear servicio `CoordinacionUserService` (`ICoordinacionService` / `CoordinacionService`) con métodos para listar usuarios institucionales, desactivar cuentas de usuarios revocando su acceso a la plataforma sin eliminar su historial de registros (`RF-05`), promover al rol `Coordinador` y revocarlo.
- [x] Diseñar vista para gestión y auditoría de cuentas con modal de confirmación y ficha técnica.
- [x] Validar que un coordinador no pueda revocar sus propios permisos de forma accidental y mínimo de coordinadores activos.

---

## 🎓 Fase 2: Solicitudes, Evaluación y Perfil Académico de Tutores (RF-06 a RF-11)

### Tarea 2.1: Solicitud de Estatus de Tutor (RF-06)
- [x] Crear ViewModel `SolicitudTutorViewModel` para que alumnos postulen a *Tutoría entre Pares* y docentes a *Tutoría Individual*.
- [x] Crear servicio `TutorSolicitudService` para registrar `SolicitudTutor` con estado inicial `Pendiente`.
- [x] Agregar validación de reglas de negocio: el usuario no debe tener ya una solicitud pendiente o el rol `Tutor` activo.
- [x] Implementar vista en el panel del alumno/docente con el estado de su solicitud actual.

### Tarea 2.2: Evaluación y Revocación de Estatus de Tutor (RF-07, RF-08)
- [x] Crear vista y acción en `CoordinacionController` para listar solicitudes pendientes con filtros por rol y fecha.
- [x] Implementar acción de **Aprobación (RF-07)**:
  - [x] Asignar rol `Tutor` mediante `UserManager.AddToRoleAsync`.
  - [x] Actualizar `TipoTutor` en `ApplicationUser` (`AlumnoTutor` o `DocenteTutor`).
  - [x] Crear registro `TutorPerfil` inicial asociado al usuario.
  - [x] Actualizar estado de solicitud a `Aprobada`.
- [x] Implementar acción de **Rechazo (RF-07)**:
  - [x] Actualizar estado a `Rechazada` y registrar mensaje oficial: *"Pasa a Coordinación para validar tu situación"*.
- [x] Implementar acción de **Suspensión / Revocación de Estatus (RF-08)**:
  - [x] Permitir a Coordinación retirar o desactivar el estatus de tutor activo, despublicando automáticamente sus horarios y perfil (`EsPublico = false`).

### Tarea 2.3: Perfil Académico del Tutor y Materias Impartidas (RF-09, RF-10, RF-11)
- [x] Crear entidad `TutorMateria` (relación N:M entre `TutorPerfil` y `Materia`) en EF Core y migración.
- [x] Crear `TutorPerfilController` protegido con `[Authorize(Roles = "Tutor")]`.
- [x] Implementar interfaz para que el tutor cree por primera vez y actualice su información académica (**RF-09, RF-10**):
  - [x] Semblanza / biografía académica (máx. 500 caracteres).
  - [x] Carrera o academia asignada.
  - [x] Selección múltiple de materias que imparte según su carrera o academia con buscador interactivo.
  - [x] Bandera `EsPublico` para activar/desactivar su visibilidad en el catálogo general.
- [x] Implementar vista previa del perfil público del tutor.
- [ ] Implementar acción de eliminación del perfil académico (**RF-11**), permitiendo al tutor eliminar la información de su perfil público del sistema.

---

## 📚 Fase 3: Horarios, Ubicaciones y Catálogo de Tutorías (RF-12 a RF-15)

### Tarea 3.1: Administración de Horarios y Ubicaciones Presenciales (RF-12, RF-14, RF-15)
- [x] Crear entidad `TutorHorario` (Día de la semana L-V, `HoraInicio`, `HoraFin`, `Lugar`/Salón asignado, `Activo`) en EF Core y migración.
- [x] Crear servicio `TutorPerfilService` con validación de no solapamiento de horarios dentro del rango escolar (07:00 a 22:00).
- [x] Implementar registro de días, horarios y salón asignado donde impartirán las sesiones (**RF-12**).
- [x] Implementar modificación de días, horarios de atención y salón/ubicación registrada (**RF-14**).
- [x] Implementar eliminación de registros específicos de días, horarios y ubicaciones (**RF-15**).

### Tarea 3.2: Catálogo de Tutorías y Búsqueda Avanzada (RF-13)
- [x] Crear `CatalogoController` protegido con `[Authorize]`, visible en la barra de navegación únicamente para usuarios autenticados.
- [x] Implementar consulta y búsqueda interactiva de tutorías y horarios disponibles (**RF-13**):
  - [x] Búsqueda por texto (nombre del tutor, materia o salón).
  - [x] Filtro por Programa Académico / Carrera.
  - [x] Filtro por Unidad de Aprendizaje / Materia.
  - [x] Filtro por Modalidad (Tutoría entre Pares y Tutoría Individual).
- [x] Diseñar tarjetas (*cards*) verticales responsivas mostrando avatar, nombre completo, carrera/academia, reputación en estrellas, materias ofertadas y botón de acceso a detalle y horarios.
- [x] Implementar vista de Detalle del Tutor (`/Catalogo/Detalle/{id}`) mostrando biografía completa, lista de materias impartidas y tabla semanal de días, horarios y salones de atención.

---

## 📲 Fase 4: Control de Asistencias Presenciales (Manual y QR Dinámico) e Historial (RF-16 a RF-20)

### Tarea 4.1: Asistencia Manual y Confirmación del Alumno (RF-16, RF-17)
- [x] Implementar búsqueda por número de boleta o nombre en el panel del Tutor para registrar asistencia manual a la sesión (**RF-16**).
- [x] Enviar solicitud de confirmación de asistencia al alumno tutorado (**RF-16**).
- [x] Crear interfaz en el perfil del alumno tutorado para visualizar la asistencia registrada por el tutor y confirmar su validez dentro del sistema (**RF-17**).
- [x] Asentar la asistencia con estado confirmado al recibir la validación del alumno.

### Tarea 4.2: Asistencia por Código QR Dinámico (RF-18, RF-19)
- [x] Crear entidad `SesionQrToken` (`SesionId`, `Token`, `FechaCreacion`, `FechaExpiracion`, `Activo`).
- [x] Implementar `QrCodeService` usando `QRCoder` para generar código QR dinámico por sesión para la validación presencial (**RF-18**).
- [x] Implementar expiración y refresco periódico del código QR para garantizar asistencia presencial estricta.
- [x] Diseñar pantalla del Tutor para proyectar el código QR con cuenta regresiva visual.
- [x] Integrar lector de cámara en la vista móvil del Alumno para escanear el código QR y confirmar su asistencia automáticamente (**RF-19**).
- [x] Validar en servidor que el token escaneado sea válido, vigente y no haya sido registrado previamente por el mismo alumno.

### Tarea 4.3: Historial de Asistencias (RF-20)
- [x] Consolidar modelo `Asistencia` detallando fecha, hora, materia impartida, ubicación/salón y método de registro (`Manual` o `CodigoQR`).
- [x] Diseñar vista para que el Alumno consulte su historial personal de asistencias registradas y confirmadas (**RF-20**).
- [x] Diseñar vista para que el Tutor consulte el reporte e historial de asistencias de las tutorías que ha impartido (**RF-20**).
- [x] Habilitar vista de auditoría y reportes (diario y mensual) para que Coordinación consulte el historial global de asistencias institucionales.

---

## ⭐ Fase 5: Encuestas de Satisfacción y Notificaciones (RF-21 a RF-24)

### Tarea 5.1: Registro de Encuestas y Consulta de Resultados (RF-21, RF-22)
- [x] Habilitar al alumno responder una encuesta de retroalimentación sobre la tutoría recibida una vez confirmada su asistencia (**RF-21**).
- [x] Diseñar componente de evaluación directa en 1 clic con calificación en estrellas (1 a 5).
- [x] Actualizar el promedio acumulado de reputación en estrellas en `TutorPerfil`.
- [x] Implementar panel en Coordinación para visualizar resultados, indicadores y áreas de oportunidad detectadas en las encuestas (**RF-22**).

### Tarea 5.2: Notificaciones Manuales y Bandeja de Notificaciones (RF-23, RF-24)
- [x] Crear módulo para que Coordinación y Tutores puedan redactar y enviar notificaciones a los alumnos tutorados (**RF-23**).
- [x] Crear entidad y servicio de `Notificacion` (`UserId`, `Titulo`, `Mensaje`, `FechaEnvio`, `Leida`).
- [x] Implementar bandeja de entrada accesible para todos los usuarios donde visualicen sus notificaciones recibidas (**RF-24**).
- [x] Integrar indicador de notificaciones pendientes/no leídas en la barra de navegación del sistema.

---

## 📊 Fase 6: Métricas, Estadísticas y Generación de Reportes PDF con QuestPDF (RF-25)

### Tarea 6.1: Tablero de Indicadores y Exportación de Reportes en PDF (RF-25)
- [ ] Crear servicio `ReportesMetricasService` con consultas optimizadas de agregación en EF Core:
  - [ ] Total de asistencias presenciales confirmadas en el periodo.
  - [ ] Índice de atención desglosado por maestro tutor vs. alumno tutor.
  - [ ] Ranking de materias con mayor demanda en UPIICSA.
  - [ ] Promedio general de satisfacción institucional derivado de encuestas.
- [ ] Diseñar tablero de indicadores estadísticos en el portal de Coordinación (**RF-25**).
- [ ] Configurar motor `QuestPDF` con paleta institucional del IPN (Guinda `#750946` y Guinda oscuro `#4d0630`).
- [ ] Diseñar plantilla oficial `ReporteTutoriasPdfDocument` con encabezado institucional, tablas detalladas y pie de página.
- [ ] Implementar endpoint de exportación y descarga de reportes estadísticos en formato PDF (**RF-25**).
