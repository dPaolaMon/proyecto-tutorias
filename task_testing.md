# 🧪 task_testing.md — Registro y Matriz de Pruebas de Software

Este documento centraliza el seguimiento con casillas de verificación (`- [ ]`) de la suite completa de pruebas para el **Sistema Web de Gestión y Visibilidad de Tutorías de UPIICSA**, cubriendo todos los requerimientos funcionales oficiales (**RF-01 al RF-25**) de `RF.txt`, desglosado por tipo de prueba y módulo del sistema.

---

## 🧩 1. Pruebas Unitarias (xUnit + Moq + FluentAssertions)

### Módulo 1: Autenticación, Usuarios e Identidad (RF-01 a RF-05)
- [ ] **RF-01 (Dominio Institucional):** Validar que correos que no pertenecen a `@ipn.mx` o `@alumno.ipn.mx` sean rechazados en el registro.
- [ ] **RF-01 (Formato de Boleta):** Validar que números de boleta contengan únicamente caracteres numéricos y longitud válida.
- [ ] **RF-04 (Recuperación de Contraseña):** Validar generación y expiración de tokens temporales de restablecimiento de contraseña.
- [ ] **RF-05 (Desactivar Usuarios):** Validar que la desactivación por Coordinación marque `Activo = false`, asiente `FechaBaja` y conserve `MotivoBaja` sin borrar el historial.
- [ ] **(Salvaguardas de Coordinación):** Validar que un coordinador no pueda revocar sus propios privilegios y se impida revocar al último coordinador activo.
- [ ] **(Paginación y Filtros):** Validar que `CoordinacionService.GetGestionUsuariosAsync` pagine exactamente a 10 registros y filtre por roles correctamente.

### Módulo 2: Solicitudes y Perfil de Tutores (RF-06 a RF-11)
- [ ] **RF-06 (Solicitar Estatus de Tutor):** Validar que alumnos postulen a *Tutoría entre Pares* y docentes a *Tutoría Individual*, impidiendo solicitudes duplicadas en estado pendiente.
- [ ] **RF-07 (Evaluar Solicitud de Tutor):** Validar que al rechazar una solicitud se asiente el mensaje institucional oficial (*"Pasa a Coordinación para validar tu situación"*).
- [ ] **RF-08 (Revocación de Estatus):** Validar que al retirar o suspender el estatus de tutor activo se desactive automáticamente la visibilidad de su perfil (`EsPublico = false`).
- [ ] **RF-09 (Crear Perfil Académico):** Validar registro inicial de biografía, materias ofertadas, carrera y academia para tutores aprobados.
- [ ] **RF-10 (Actualizar Perfil Académico):** Validar límites de captura en semblanza (máx. 500 caracteres) y salón asignado (máx. 50 caracteres).
- [ ] **RF-11 (Eliminar Perfil Académico):** Validar retiro de datos del perfil público del tutor despublicándolo del catálogo.

### Módulo 3: Horarios, Disponibilidad y Catálogo (RF-12 a RF-15)
- [ ] **RF-12 (Registrar Horarios y Ubicación):** Validar que el servicio rechace bloques horarios que se traslapen en el mismo día para un tutor y asigne salón/espacio.
- [ ] **RF-13 (Consultar Horarios y Tutorías):** Validar filtrado dinámico de horarios y tutorías disponibles por unidad de aprendizaje, carrera y ubicación.
- [ ] **RF-14 (Actualizar Horarios):** Validar modificación de días, rangos de horas y espacio asignado de tutorías existentes.
- [ ] **RF-15 (Eliminar Horarios):** Validar eliminación de bloques horarios específicos liberando la agenda del tutor.
- [ ] **(Regla de Capacidad Docente):** Validar restricción de cupo de atención estricta de máximo 10 alumnos para tutores docentes.

### Módulo 4: Asistencias y Códigos QR (RF-16 a RF-20)
- [ ] **RF-16 (Registrar Asistencia Manual):** Validar búsqueda de alumno por boleta o nombre y despacho de petición de confirmación.
- [ ] **RF-17 (Confirmar Asistencia Manual):** Validar que el estado de asistencia cambie a confirmado una vez que el alumno acepta la solicitud.
- [ ] **RF-18 (Generar Código QR):** Validar generación de token dinámico con `QrCodeService` y expiración estricta de 20 minutos por sesión.
- [ ] **RF-19 (Tomar Asistencia por QR):** Validar validación criptográfica del token QR escaneado y bloqueo de asistencias duplicadas por sesión.
- [ ] **RF-20 (Consultar Historial de Asistencias):** Validar consulta de asistencias registradas con detalles de fecha, hora, materia y salón.

### Módulo 5: Encuestas, Notificaciones y Métricas (RF-21 a RF-25)
- [ ] **RF-21 (Registrar Encuesta):** Validar que solo alumnos con asistencia confirmada puedan responder la encuesta de 1 a 5 estrellas (comentarios máx. 300 caracteres).
- [ ] **RF-22 (Consultar Resultados de Encuesta):** Validar cálculo de promedios de satisfacción cuantitativos y agregación de retroalimentación cualitativa para Coordinación.
- [ ] **RF-23 (Enviar Notificaciones Manuales):** Validar despacho de avisos personalizados desde Coordinación o Tutores hacia los alumnos.
- [ ] **RF-24 (Consultar Notificaciones):** Validar marcado de notificaciones leídas y actualización del contador de pendientes.
- [ ] **RF-25 (Generar Métricas):** Validar agregación estadística de asistencias presenciales confirmadas, índice de atención (docente vs. alumno tutor) y ranking de materias.

---

## 🔗 2. Pruebas de Integración (WebApplicationFactory + EF Core)

### Módulo 1: Autenticación, Acceso y Gestión de Cuentas (RF-01 a RF-05)
- [ ] **RF-01 (Registro y Correo):** Ejecutar `POST /Account/Register` y verificar creación de usuario en base de datos con `EmailConfirmed = false`.
- [ ] **RF-01 (Confirmación de Correo):** Invocar `GET /Account/ConfirmEmail` con token válido y comprobar actualización en base de datos y sesión activa.
- [ ] **RF-02 (Iniciar Sesión):** Validar inicio de sesión exitoso con credenciales correctas y rechazo ante correo no verificado.
- [ ] **RF-03 (Cerrar Sesión):** Validar invalidación de cookies de autenticación tras invocar `POST /Account/Logout`.
- [ ] **RF-04 (Recuperar Contraseña):** Invocar flujo de `ForgotPassword` y comprobar que no se exponga si un correo existe o no en el sistema.
- [ ] **RF-05 (Desactivar Usuarios):** Ejecutar desactivación de cuenta desde Coordinación y validar que no pueda iniciar sesión posteriormente.
- [ ] **(Autorización de Coordinador):** Validar que usuarios sin rol `Coordinador` reciban `403 Forbidden` / redirección a `AccessDenied` al acceder a `/Coordinacion/Index`.

### Módulo 2 y 3: Solicitudes, Perfil y Catálogo (RF-06 a RF-15)
- [ ] **RF-06 (Solicitar Estatus de Tutor):** Ejecutar `POST` de postulación a tutoría y verificar inserción en tabla de solicitudes con estado `Pendiente`.
- [ ] **RF-07 (Evaluar Solicitud de Tutor):** Validar que la aprobación cree transaccionalmente el rol `Tutor` y el registro `TutorPerfil`.
- [ ] **RF-08 (Revocación de Estatus):** Ejecutar suspensión de tutor y verificar en base de datos `EsPublico = false`.
- [ ] **RF-09 (Crear Perfil Académico):** Guardar perfil inicial del tutor y verificar persistencia en `TutoresPerfiles`.
- [ ] **RF-10 (Actualizar Perfil Académico):** Modificar campos de semblanza y materias y verificar actualización en base de datos.
- [ ] **RF-11 (Eliminar Perfil Académico):** Retirar información pública del tutor y verificar actualización en base de datos.
- [ ] **RF-12 (Registrar Horarios y Ubicación):** Crear bloque de horario y verificar persistencia relacional con el salón.
- [ ] **RF-13 (Consultar Horarios y Tutorías):** Invocar `GET /Catalogo/Index` de forma anónima y verificar respuesta con tutorías ofertadas.
- [ ] **RF-14 (Actualizar Horarios):** Modificar horario existente vía `POST` y corroborar actualización en EF Core.
- [ ] **RF-15 (Eliminar Horarios):** Ejecutar baja de horario vía `POST` y confirmar eliminación lógica o física.

### Módulo 4 y 5: Asistencias, Encuestas y Métricas (RF-16 a RF-25)
- [ ] **RF-16 (Registrar Asistencia Manual):** Ejecutar registro manual en controlador y validar estado inicial pendiente de confirmación.
- [ ] **RF-17 (Confirmar Asistencia Manual):** Ejecutar confirmación del alumno y verificar `Confirmada = true` en base de datos.
- [ ] **RF-18 (Generar Código QR):** Validar endpoint de emisión de QR retornando token encriptado y fecha de vigencia.
- [ ] **RF-19 (Tomar Asistencia por QR):** Enviar petición de escaneo de QR y verificar inserción de registro en `Asistencias`.
- [ ] **RF-20 (Consultar Historial de Asistencias):** Consultar endpoint de historial y verificar filtro estricto por usuario autenticado.
- [ ] **RF-21 (Registrar Encuesta):** Enviar formulario de evaluación y validar inserción en tabla de encuestas.
- [ ] **RF-22 (Consultar Resultados de Encuesta):** Acceder a bandeja de resultados con rol Coordinador y validar métricas consolidadas.
- [ ] **RF-23 (Enviar Notificaciones Manuales):** Crear notificación dirigida y corroborar inserción en base de datos.
- [ ] **RF-24 (Consultar Notificaciones):** Invocar endpoint de bandeja de notificaciones y comprobar marcado de leída.
- [ ] **RF-25 (Generar Métricas en PDF):** Realizar petición autenticada a `GET /Coordinacion/ExportarReportePdf` y verificar `Content-Type: application/pdf` con stream generado por QuestPDF.

---

## 🌐 3. Pruebas Funcionales E2E con Selenium WebDriver

### Flujos de Autenticación, Perfil y Gestión
- [ ] **E2E-01 (RF-01, RF-02, RF-03):** Flujo completo de registro institucional, confirmación por enlace, inicio de sesión y cierre seguro de sesión.
- [ ] **E2E-02 (RF-04):** Solicitud de restablecimiento de contraseña en `ForgotPassword` e ingreso de nueva contraseña.
- [ ] **E2E-03 (RF-05):** Coordinador desactivando un usuario desde la tabla de usuarios y verificación de bloqueo de acceso.
- [ ] **E2E-04 (Gestión de Usuarios):** Interacción con barra de búsqueda, paginación a 10 registros y apertura de modal `#modalFichaUsuario`.

### Flujos de Tutores y Catálogo
- [ ] **E2E-05 (RF-06, RF-07):** Postulación de alumno como tutor, revisión en panel de Coordinación y aprobación.
- [ ] **E2E-06 (RF-09, RF-10):** Captura de perfil académico del tutor, asignación de salón y actualización de semblanza.
- [ ] **E2E-07 (RF-12, RF-13):** Registro de horarios por el tutor y visualización inmediata en el catálogo público con tarjetas responsivas.
- [ ] **E2E-08 (RF-14, RF-15):** Modificación y eliminación de bloques horarios desde el panel del tutor.

### Flujos de Asistencias, Encuestas y Notificaciones
- [ ] **E2E-09 (RF-18, RF-19):** Panel del tutor proyectando código QR con temporizador y simulación de escaneo por alumno.
- [ ] **E2E-10 (RF-16, RF-17):** Registro de asistencia manual por el tutor y confirmación interactiva en modal del alumno.
- [ ] **E2E-11 (RF-21, RF-22):** Alumno calificando la sesión con estrellas tras la tutoría y Coordinación consultando resultados.
- [ ] **E2E-12 (RF-23, RF-24):** Envío de aviso por el tutor y visualización en la campanita de notificaciones del alumno.
- [ ] **E2E-13 (RF-25):** Coordinador accediendo al panel de estadísticas y descargando el reporte generado en PDF.

---

## ⚡ 4. Pruebas de Carga y Rendimiento (Apache JMeter)

### Escenario A: Escaneo Masivo y Concurrente de Asistencia QR (RF-19)
- [ ] **JMX-01:** Plan de prueba con 50 hilos concurrentes consumiendo `/Asistencia/RegistrarQr`.
- [ ] **JMX-01-METRIC:** Tiempo de respuesta $p_{95} < 500$ ms, tasa de error $< 0.1\%$.

### Escenario B: Consultas Concurrentes al Catálogo de Tutorías (RF-13)
- [ ] **JMX-02:** Plan de prueba con 100 hilos concurrentes aplicando filtros en `/Catalogo/Index`.
- [ ] **JMX-02-METRIC:** Throughput sostenido $> 50$ req/s, $p_{95} < 300$ ms.

### Escenario C: Autenticación Concurrente Masiva (RF-02)
- [ ] **JMX-03:** Plan de prueba con 50 logins simultáneos (`POST /Account/Login`).
- [ ] **JMX-03-METRIC:** Sin errores 500 ni saturación en conexiones SQL Server.

### Escenario D: Descarga Concurrente de Reportes PDF (RF-25)
- [ ] **JMX-04:** Plan de prueba con peticiones concurrentes a exportación PDF.
- [ ] **JMX-04-METRIC:** Generación exitosa de documentos con uso estable de memoria en el servidor.
