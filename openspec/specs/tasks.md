# 📋 tasks.md — Plan de Tareas Incrementales de Desarrollo

Este documento desglosa la implementación integral del **Sistema de Tutorías Académicas** en fases incrementales y verificables, basado en los requerimientos funcionales (`requirements.md`), el flujo oficial (`REQUISITOS_Y_FLUJO_SISTEMA.md`), la arquitectura (`ARCHITECTURE.md`) y el modelo de datos (`data-model.md`).

---

## 🚀 Fase 1: Autenticación, Verificación de Cuentas y Gestión de Identidad (RF-01 a RF-04, RF-31, RF-32)

### Tarea 1.1: Base de Datos, Catálogo UPIICSA y Modelado Base de Usuario
- [x] Crear entidad `ApplicationUser` extendiendo `IdentityUser` con `Nombre`, `ApellidoPaterno`, `ApellidoMaterno`, `Boleta` (solo dígitos), `CarreraId`, `Academia` y banderas de baja lógica (`Activo`, `FechaBaja`, `MotivoBaja`).
- [x] Crear entidades `Carrera`, `Materia` y `MateriaEquivalencia` con Seeder automático desde `upiicsa_catalogo.json`.
- [x] Implementar vistas responsivas de Login, Registro, Completar Perfil, Edición de Perfil y Baja Lógica.
- [x] Generar y aplicar migraciones iniciales de EF Core a la base de datos SQL Server / Azure SQL.

### Tarea 1.2: Verificación de Correo Institucional en Desarrollo y Producción (RF-01)
- [x] Diseñar interfaz `IEmailSenderService` y registrar dos implementaciones: `DevEmailService` (desarrollo, logger en consola y enlace de depuración visual) y `AzureEmailService` (producción vía Azure Communication Services).
- [x] Configurar ASP.NET Core Identity con `options.SignIn.RequireConfirmedEmail = true`.
- [x] Modificar `AccountController.Register` para crear el usuario con `EmailConfirmed = false`, generar el token con `UserManager.GenerateEmailConfirmationTokenAsync` y enviar el enlace sin iniciar sesión directo.
- [x] Implementar vista `RegisterConfirmation.cshtml` con mensaje institucional y botón de depuración en modo local (`@if (appEnv.IsDevelopment())`).
- [x] Implementar acción `GET /Account/ConfirmEmail` que valide `userId` y `token`, asiente la confirmación con `ConfirmEmailAsync`, inicie sesión automáticamente y redirija a la Pantalla de Inicio (`/Home/Index`).
- [x] Bloquear inicio de sesión en `AccountController.Login` si `EmailConfirmed == false` y proveer flujo de reenvío con `ResendConfirmationEmail`.

### Tarea 1.3: Recuperación de Contraseña (RF-31)
- [x] Implementar `ForgotPasswordViewModel` solicitando correo institucional (`@alumno.ipn.mx` o `@ipn.mx`).
- [x] Crear acciones `ForgotPassword` y `ResetPassword` en `AccountController` usando `GeneratePasswordResetTokenAsync` y `ResetPasswordAsync`.
- [x] Diseñar vistas Razor `ForgotPassword.cshtml`, `ForgotPasswordConfirmation.cshtml` y `ResetPassword.cshtml`.
- [x] Verificar que no se revele la existencia de correos no registrados al solicitar restablecimiento.

### Tarea 1.4: Asignación y Promoción del Rol Coordinación (RF-32)
- [x] Crear `CoordinacionController` protegido con `[Authorize(Roles = "Coordinador")]`.
- [x] Crear servicio `CoordinacionUserService` (`ICoordinacionService` / `CoordinacionService`) con métodos para listar usuarios institucionales, promover al rol `Coordinador` y revocarlo.
- [x] Diseñar vista para gestión de roles administrativos con modal de confirmación y ficha técnica en modal.
- [x] Validar que un coordinador no pueda revocar sus propios permisos de forma accidental y mínimo de coordinadores.

---

## 🎓 Fase 2: Solicitudes, Aprobación y Perfil Académico de Tutores (RF-05 a RF-12)

### Tarea 2.1: Solicitud de Estatus de Tutor (RF-05, RF-06)
- [ ] Crear ViewModel `SolicitudTutorViewModel` para que alumnos postulen a *Tutoría entre Pares* y docentes a *Tutoría Individual*.
- [ ] Crear servicio `TutorSolicitudService` para registrar `SolicitudTutor` con estado inicial `Pendiente`.
- [ ] Agregar validación de reglas de negocio: el usuario no debe tener ya una solicitud pendiente o el rol `Tutor` activo.
- [ ] Implementar vista en el panel del alumno/docente con el estado de su solicitud actual.

### Tarea 2.2: Bandeja de Coordinación para Aprobación/Rechazo de Tutores (RF-06, RF-07, RF-08)
- [ ] Crear vista y acción en `CoordinacionController` para listar solicitudes pendientes con filtros por rol y fecha.
- [ ] Implementar acción de **Aprobación**:
  - [ ] Asignar rol `Tutor` mediante `UserManager.AddToRoleAsync`.
  - [ ] Actualizar `TipoTutor` en `ApplicationUser` (`AlumnoTutor` o `DocenteTutor`).
  - [ ] Crear registro `TutorPerfil` inicial asociado al usuario.
  - [ ] Actualizar estado de solicitud a `Aprobada`.
- [ ] Implementar acción de **Rechazo**:
  - [ ] Actualizar estado a `Rechazada` y registrar mensaje oficial: *"Pasa a Coordinación para validar tu situación"*.
- [ ] Implementar acción de **Suspensión / Revocación**:
  - [ ] Permitir a Coordinación suspender el rol de tutor, despublicando automáticamente sus horarios y perfil (`EsPublico = false`).

### Tarea 2.3: Perfil Público Académico del Tutor y Materias Impartidas (RF-09, RF-10, RF-11, RF-12)
- [ ] Crear entidad `TutorMateria` (relación N:M entre `TutorPerfil` y `Materia`) en EF Core y migración.
- [ ] Crear `TutorPerfilController` protegido con `[Authorize(Roles = "Tutor")]`.
- [ ] Implementar interfaz para que el tutor capture:
  - [ ] Biografía / semblanza académica (máx. 500 caracteres).
  - [ ] Salón asignado (máx. 50 caracteres).
  - [ ] Selección múltiple de materias que imparte según su carrera o academia.
  - [ ] Bandera `EsPublico` para activar/desactivar su visibilidad en el catálogo general.
- [ ] Implementar vista previa del perfil público del tutor.

---

## 📚 Fase 3: Disponibilidad, Horarios y Catálogo de Tutorías (RF-13 a RF-17)

### Tarea 3.1: Administración de Horarios y Disponibilidad Presencial (RF-13, RF-15, RF-16)
- [ ] Crear entidad `TutorDisponibilidad` (Día de la semana, `HoraInicio`, `HoraFin`, `Lugar`/Salón, `Activo`) en EF Core y migración.
- [ ] Crear servicio `TutorHorarioService` con validación de no solapamiento de horarios para el mismo tutor.
- [ ] Implementar regla de capacidad:
  - [ ] Docente Tutor (*Tutoría Individual*): restricción de cupo estricto a **10 alumnos** inscritos/atendidos.
  - [ ] Alumno Tutor (*Tutoría entre Pares*): capacidad delimitada por el cupo físico del salón o tutor.
- [ ] Implementar CRUD responsivo (Alta, Edición, Baja) de bloques horarios desde el panel del tutor.

### Tarea 3.2: Catálogo Público de Tutorías y Búsqueda Avanzada (RF-14, RF-17)
- [ ] Crear `CatalogoController` con acceso público para usuarios autenticados y no autenticados.
- [ ] Implementar filtros multicriterio dinámicos:
  - [ ] Por Unidad de Aprendizaje / Materia.
  - [ ] Por Nombre de Tutor.
  - [ ] Por Programa Académico / Carrera.
  - [ ] Por Modalidad (Pares, Individual Docente, Especial Coordinación).
- [ ] Diseñar tarjetas (*cards*) responsivas mostrando foto, nombre, carrera/academia, reputación en estrellas, materias ofertadas, horarios y salón.
- [ ] Gestionar **Tutorías Especiales de Coordinación**:
  - [ ] Habilitar alta de tutorías informativas por parte de Coordinación.
  - [ ] Marcarlas en el catálogo con etiqueta informativa (sin botones de asistencia ni encuestas).

---

## 📲 Fase 4: Registro y Control de Asistencias (QR Dinámico TOTP + SignalR) (RF-18 a RF-23)

### Tarea 4.1: Modelo y Generación de Códigos QR Dinámicos (RF-20, RF-21)
- [ ] Crear entidad `SesionQrToken` (`SesionId`, `TokenHash`, `FechaCreacion`, `FechaExpiracion`, `Activo`).
- [ ] Implementar `QrCodeService` usando la librería `QRCoder` para emitir tokens cifrados de asistencia.
- [ ] Implementar expiración estricta de **20 minutos** por token QR dinámico.
- [ ] Diseñar pantalla del Tutor para proyectar el código QR con cuenta regresiva visual y refresco automático al vencer los 20 minutos.
- [ ] Integrar librería cliente `html5-qrcode` en la vista móvil del Alumno para escanear el QR desde el navegador del dispositivo.
- [ ] Validar en servidor que el token escaneado esté vigente, no haya sido consumido previamente por el mismo alumno y corresponda a la sesión activa.

### Tarea 4.2: Asistencia Manual y Confirmación en Tiempo Real con SignalR (RF-18, RF-19)
- [ ] Configurar `SignalR` en `Program.cs` y crear `AsistenciaHub`.
- [ ] Implementar búsqueda por boleta o nombre en el panel del Tutor para registrar asistencia manual presencial.
- [ ] Despachar notificación en tiempo real vía WebSocket al dispositivo del Alumno cuando el tutor registre su asistencia manual.
- [ ] Crear modal de confirmación en la vista del Alumno (*"¿Confirmas tu asistencia a la tutoría de [Materia] impartida por [Tutor]?"*).
- [ ] Asentar la asistencia con estado `Confirmada = true` al recibir la respuesta del alumno.

### Tarea 4.3: Registro e Historial de Asistencias (RF-22, RF-23)
- [ ] Consolidar en `Asistencia`: Alumno, Tutor, Sesión, Materia, Fecha/Hora, Método (`CodigoQR` o `ManualSignalR`).
- [ ] Implementar vistas de historial con permisos estrictos de servidor:
  - [ ] **Alumno:** Solo visualiza sus asistencias acreditadas personales.
  - [ ] **Tutor:** Visualiza las asistencias de sus sesiones impartidas con opción de exportar lista.
  - [ ] **Coordinación:** Consulta el historial institucional consolidado con filtros de fecha y materia.

---

## ⭐ Fase 5: Encuestas de Satisfacción, Reputación y Notificaciones In-App (RF-24 a RF-28)

### Tarea 5.1: Encuestas de Satisfacción y Cálculo de Reputación (RF-24, RF-25)
- [ ] Validar que la encuesta de retroalimentación se habilite **únicamente** para alumnos con asistencia previamente confirmada tras concluir el horario de la sesión.
- [ ] Diseñar componente de evaluación interactivo de 1 a 5 estrellas con campo de comentarios opcional (máx. 300 caracteres).
- [ ] Implementar cálculo acumulativo del promedio de estrellas y total de evaluaciones en `TutorPerfil`.
- [ ] Mostrar estrellas y calificación promedio en el Perfil Público del Tutor dentro del catálogo.
- [ ] Crear vista para que el Tutor y Coordinación analicen el feedback cualitativo y cuantitativo recibido.

### Tarea 5.2: Bandeja de Notificaciones In-App (RF-26, RF-27, RF-28)
- [ ] Crear entidad `Notificacion` (`UserId`, `Titulo`, `Mensaje`, `Leida`, `FechaEnvio`, `Tipo`, `UrlAccion`).
- [ ] Crear servicio `NotificationService` para dispatch de avisos automáticos ante:
  - [ ] Aprobación o rechazo de solicitud de tutor.
  - [ ] Solicitud de confirmación de asistencia presencial.
  - [ ] Activación de encuesta de retroalimentación pendiente.
- [ ] Implementar funcionalidad para que Coordinación y Tutores redacten avisos a los alumnos tutorados.
- [ ] Crear componente de campanita / bandeja en el `_Layout.cshtml` con contador de notificaciones no leídas y marcado como leída.

---

## 📊 Fase 6: Métricas, Estadísticas y Generación de Reportes PDF con QuestPDF (RF-29, RF-30)

### Tarea 6.1: Tablero de Métricas e Indicadores de Coordinación (RF-29)
- [ ] Crear servicio `ReportesMetricasService` con consultas optimizadas de agregación en EF Core:
  - [ ] Total de asistencias presenciales confirmadas en el periodo.
  - [ ] Índice de atención desglosado por docente tutor vs. alumno tutor.
  - [ ] Ranking de materias con mayor demanda de tutorías en UPIICSA.
  - [ ] Promedio general de satisfacción institucional.
- [ ] Diseñar panel de métricas con tarjetas visuales responsivas y gráficas estadísticas en el portal de Coordinación.

### Tarea 6.2: Motor de Exportación en PDF con QuestPDF (RF-30)
- [ ] Configurar `QuestPDF` con tipografías y paleta institucional del IPN (Guinda `#750946`, Guinda oscuro `#4d0630`).
- [ ] Crear plantilla de documento `ReporteTutoriasPdfDocument`:
  - [ ] Encabezado oficial con logotipos institucionales del IPN y UPIICSA.
  - [ ] Tabla detallada de asistencias y resumen ejecutivo de métricas.
  - [ ] Pie de página con numeración y fecha/hora de generación.
- [ ] Implementar endpoint `GET /Coordinacion/ExportarReportePdf` retornando `application/pdf` procesado en el servidor.
- [ ] Verificar compatibilidad de renderizado de fuentes tanto en Linux (Docker) como en Windows.

---

## 🧪 Fase 7: Validación de Calidad y Pruebas de Carga con Apache JMeter

### Tarea 7.1: Planes de Prueba JMeter (.jmx)
- [ ] Diseñar plan de prueba JMeter para endpoints de alta concurrencia:
  - [ ] Escaneo masivo concurrente de códigos QR de asistencia (`/Asistencia/RegistrarQr`).
  - [ ] Autenticación y consultas concurrentes al Catálogo público (`/Catalogo/Index`).
- [ ] Ejecutar plan de pruebas y verificar tiempos de respuesta aceptables bajo carga.
- [ ] Confirmar compilación limpia sin advertencias (`dotnet build`) y esquema de migraciones íntegro (`dotnet ef database update`).
