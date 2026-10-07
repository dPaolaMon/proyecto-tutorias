# 📜 Requisitos y Flujo Completo del Sistema de Tutorías

Este documento detalla todas las reglas de negocio, flujos de usuario y especificaciones del **Sistema de Tutorías Académicas**, sirviendo como especificación funcional del proyecto.

---

## 🔐 1. Registro, Autenticación y Verificación de Usuarios

1. **Registro Institucional:**
   * El usuario (Alumno) se registra ingresando su **correo institucional** (ej. `@alumnos.edu.mx`).
   * El sistema valida la pertenencia al dominio de la institución.
2. **Confirmación de Correo:**
   * El sistema envía un token de confirmación por correo electrónico (usando Azure Communication Services).
   * Al confirmar el correo, el usuario obtiene el **Rol inicial con mínimos permisos: `Alumno`**.

---

## 🎓 2. Solicitud y Aprobación de Tutores (Entre Pares y Docentes)

1. **Solicitud de Tutor:**
   * Un `Alumno` que desea ser **Alumno Tutor** (programa de tutoría entre pares) envía una solicitud desde su perfil.
   * Un **Docente** que dará tutorías individuales también debe enviar una solicitud de aprobación como `Docente Tutor`.
2. **Validación por la Coordinación:**
   * La **Coordinación de Tutorías** (Admin) revisa las solicitudes en su panel de administración.
   * **Aprobación:** Si responde *"Sí, es alumno/docente tutor"*, el usuario obtiene los privilegios del rol `Tutor`.
   * **Rechazo:** Si responde *"No es tutor"*, el estado se actualiza y al usuario se le notifica el mensaje: *"Pasa a Coordinación para validar tu situación"*.
3. **Creación de Perfil Público:**
   * Al ser aprobado, el `Tutor` puede configurar y publicar su **Perfil Público** (fotografía, biografía, materias que imparte, catálogo de horarios y salón asignado).
   * Al activar `EsPublico = true`, el perfil aparece en el **Catálogo de Tutorías**.

---

## 📚 3. Clasificación de Tutorías y Reglas de Capacidad

| Tipo de Tutoría | Impartido Por | Proceso de Alta / Aprobación | Capacidad / Restricción | Funcionalidades Habilitadas |
| :--- | :--- | :--- | :--- | :--- |
| **Tutoría entre Pares** | Alumno Tutor | Solicitud aprobada por Coordinación | Capacidad definida por el tutor / salón | Calendario, Asistencia (QR/Manual), Encuestas, Reputación. |
| **Tutoría Individual** | Docente Tutor | Solicitud aprobada por Coordinación | **Máximo 10 alumnos inscritos por tutoría** | Calendario, Asistencia (QR/Manual), Encuestas, Reputación. |
| **Tutoría Especial** *(ej. Regularización)* | Coordinación | Creadas y publicadas directamente por la Coordinación | N/A | **Únicamente visibilidad en el Catálogo** (sin control de asistencias ni encuestas). |

---

## 📅 4. Calendario y Disponibilidad del Tutor

* Cada `Tutor` administra un calendario donde especifica:
  * **Lugar / Salón** asignado.
  * **Materia o tema** de la tutoría.
  * **Días y Horarios** de atención.

---

## ⏱️ 5. Control y Registro de Asistencia (Módulos de Lista)

El sistema soporta dos métodos para pasar lista durante la sesión:

### Opción A: Código QR Dinámico (TOTP)
1. El `Tutor` genera/proyecta desde la app un código QR.
2. Para evitar asistencias compartidas o capturas de pantalla fuera del aula, **el código QR se refresca automáticamente cada 20 minutos**.
3. El `Alumno` escanea el QR desde su dispositivo móvil.
4. El sistema registra automáticamente la asistencia extrayendo los datos del alumno: Nombre, Carrera, Boleta, Materia, Fecha y Hora.

### Opción B: Asistencia Manual + Confirmación in-App
1. El `Tutor` busca al alumno por su nombre dentro de la app.
2. El sistema envía una **solicitud de confirmación de asistencia en tiempo real** (SignalR) a la sección de notificaciones del alumno.
3. El alumno confirma la notificación desde su móvil para validar la asistencia.

---

## ⭐ 6. Encuestas de Satisfacción y Reputación del Tutor

1. **Disparo de Encuesta:** Cuando el sistema detecta que concluyó el horario de la tutoría, envía una notificación in-app a los alumnos que registraron asistencia.
2. **Evaluación:** El alumno responde una encuesta corta puntuando la sesión de **1 a 5 estrellas** con comentarios opcionales.
3. **Reputación Pública:** El promedio de estrellas se refleja públicamente en el perfil del tutor dentro del catálogo de tutorías.

---

## 📊 7. Consultas, Reportes y Dashboard de Métricas

* **Vistas para el Tutor:** Puede consultar el historial completo de asistencias de sus propias tutorías.
* **Vistas para el Alumno Tutorado:** 
  * Explorar catálogo de tutorías.
  * Escanear QR / Confirmar asistencia.
  * Gestionar su perfil personal.
  * Solicitar ser tutor.
  * Recibir notificaciones in-app.
* **Vistas Exclusivas de la Coordinación:**
  * Acceso a asistencias globales de todos los tutores.
  * **Dashboard de Métricas & Analytics:** Asistencia por carrera/materia, tutores más valorados, toma de decisiones institucionales.
  * Exportación de reportes oficiales en formato **PDF (QuestPDF)**.
