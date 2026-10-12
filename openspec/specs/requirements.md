# Especificacion de Requerimientos Funcionales (SRS) — Sistema Web de Gestion y Visibilidad de Tutorias para UPIICSA

## 1. Informacion General y Alcance
* **Proyecto:** Sistema Web de Gestion y Visibilidad de Tutorias para UPIICSA.
* **Institucion:** Unidad Profesional Interdisciplinaria de Ingenieria y Ciencias Sociales y Administrativas (UPIICSA - IPN).
* **Objetivo:** Facilitar la difusion, gestion, control presencial y seguimiento estadistico de las tutorias presenciales entre pares (alumnos) y docentes, administradas por la Coordinacion de Tutorias.

---

## Modulo 1: Autenticacion, Acceso y Gestion de Cuentas

### RF-01: Registro de Usuarios
* **Descripcion:** Permitir el registro de nuevos usuarios en la plataforma.
* **Reglas de Negocio y Criterios:**
  * El registro se restringe de forma estricta a cuentas con dominio institucional.
  * El sistema envia un token de confirmacion transaccional por correo electronico mediante Azure Communication Services.
  * Al completar la verificacion del correo, el usuario adquiere inicialmente el rol Alumno o Docente dependiendo del dominio del correo (@alumno.ipn.mx para Alumnos y @ipn.mx para docentes).

### RF-02: Inicio de Sesion
* **Descripcion:** Permitir a los usuarios iniciar sesion en el sistema.
* **Reglas de Negocio y Criterios:**
  * Autenticacion mediante correo institucional y contrasenia administrada por ASP.NET Core Identity.
  * Manejo de sesion activa mediante cookies seguras.

### RF-03: Cierre de Sesion
* **Descripcion:** Permitir a los usuarios finalizar su sesion activa.
* **Reglas de Negocio y Criterios:**
  * Invalidacion inmediata de las cookies de autenticacion del usuario.

### RF-04: Baja de Usuarios
* **Descripcion:** Permitir dar de baja a usuarios del sistema.
* **Reglas de Negocio y Criterios:**
  * Desde el perfil de usuario, permitir dar de baja tu cuenta con la opcion de volver a dar de alta

### RF-31: Recuperacion de Contrasenia
* **Descripcion:** Permitir a los usuarios solicitar el restablecimiento de su contrasenia en caso de olvido.
* **Reglas de Negocio y Criterios:**
  * Solicitud mediante ingreso del correo institucional registrado.
  * Emision de un token temporal de restablecimiento enviado por correo electronico.

### RF-32: Asignación y Promoción del Rol Coordinación

- **Descripción:** Permitir al rol de Coordinación promover a otros usuarios registrados con correo institucional al rol `Coordinacion`, así como revocar dichos permisos.
- **Reglas de Negocio:** Solo un usuario autenticado con rol `Coordinacion` puede acceder a este módulo y cambiar los roles administrativos de otros usuarios.

---

## Modulo 2: Estatus, Aprobacion y Revocacion de Tutores

### RF-05: Solicitud de Estatus de Tutor
* **Descripcion:** Permitir a los docentes y alumnos enviar una solicitud a Coordinacion para obtener el estatus de tutor activo.
* **Reglas de Negocio y Criterios:**
  * El usuario postula indicando la modalidad: Tutoria entre Pares (alumno) o Tutoria Individual (docente).

### RF-06: Recepcion de Solicitud Tutor
* **Descripcion:** Permitir al rol de Coordinacion recibir y visualizar solicitudes de estatus de tutor de alumnos y docentes en su panel.

### RF-07: Respuesta a Solicitud de Estatus Tutor
* **Descripcion:** Permitir al rol de Coordinacion aceptar o rechazar las solicitudes de alta de tutores.
* **Reglas de Negocio y Criterios:**
  * Aprobacion: Asigna privilegios del rol Tutor y habilita la creacion del perfil academico y calendario.
  * Rechazo: Notifica al solicitante el mensaje: "Pasa a Coordinacion para validar tu situacion".

### RF-08: Revocacion o Suspension de Estatus
* **Descripcion:** Permitir a la Coordinacion suspender o dar de baja el estatus de un tutor.
* **Reglas de Negocio y Criterios:**
  * La suspension o revocacion inhabilita la publicacion de los horarios y del perfil en el catalogo.

---

## Modulo 3: Perfil Publico Academico del Tutor

### RF-09: Edicion de Datos de Usuario (Tutores)
* **Descripcion:** Permitir a los tutores aprobados editar y actualizar su perfil publico (materias que imparten y la carrera o academia correspondiente).

### RF-10: Creacion del Perfil Publico Academico
* **Descripcion:** Permitir a los tutores aprobados crear su perfil publico.
* **Reglas de Negocio y Criterios:**
  * Requiere fotografia, biografia, unidades de aprendizaje impartidas, carrera y academia.
  * Incluye bandera EsPublico para controlar la aparicion en el Catalogo de Tutorias.

### RF-11: Actualizacion de Perfil Publico Academico
* **Descripcion:** Permitir a los tutores aprobados editar la informacion de su perfil publico en cualquier momento[.

### RF-12: Eliminacion del Perfil Publico Academico
* **Descripcion:** Permitir a los tutores aprobados eliminar o despublicar su perfil publico del sistema.

---

## Modulo 4: Horarios, Ubicacion y Catalogo de Tutorias

### RF-13: Registro de Horarios y Ubicacion Presencial
* **Descripcion:** Permitir al tutor registrar sus horarios de atencion y la ubicacion fisica donde se impartira la tutoria[.
* **Reglas de Negocio y Criterios:**
  * Requiere especificacion obligatoria de salon asignado, materia, dias y horarios.
  * Para Tutoria Individual (docente), se restringe a un maximo estricto de 10 alumnos inscritos por tutoria.
  * Para Tutoria entre Pares (alumno), la capacidad se delimita por el salon o tutor.

### RF-14: Consulta de Horarios de Tutorias
* **Descripcion:** Permitir a cualquier usuario, independientemente de su rol, consultar los horarios y lugares de tutoria disponibles.

### RF-15: Actualizacion de Horarios y Ubicacion Presencial
* **Descripcion:** Permitir a los tutores modificar sus horarios de atencion y salon o ubicacion registrados.

### RF-16: Eliminacion de Horarios y Ubicacion Presencial
* **Descripcion:** Permitir a los tutores eliminar registros de bloques horarios y ubicacion que ya no atiendan.

### RF-17: Busqueda y Filtros de Tutorias
* **Descripcion:** Permitir la busqueda y filtrado de tutorias por unidad de aprendizaje, nombre del tutor o programa academico / carrera.
* **Reglas de Negocio y Criterios:**
  * Las Tutorias Especiales registradas por Coordinacion se muestran en el catalogo exclusivamente como elementos informativos (sin modulos de asistencia ni encuestas).

---

## Modulo 5: Control y Registro de Asistencia Presencial

### RF-18: Toma de Asistencia Manual
* **Descripcion:** Permitir al tutor buscar por numero de boleta o nombre y registrar manualmente la asistencia de los alumnos presentes.

### RF-19: Confirmacion de Asistencia Manual del Alumno
* **Descripcion:** Permitir al alumno tutorado verificar y confirmar el registro de asistencia realizado por el tutor.
* **Reglas de Negocio y Criterios:**
  * Despacho de notificacion en tiempo real mediante SignalR al dispositivo del alumno.
  * El alumno valida la solicitud desde la interfaz movil para asentar la asistencia.

### RF-20: Toma de Asistencia QR para Tutor
* **Descripcion:** Permitir a los tutores generar un codigo QR dinamico para que los tutorados lo escaneen y confirmen asistencia.
* **Reglas de Negocio y Criterios:**
  * Emision basada en TOTP con QRCoder.
  * El codigo QR se refresca automaticamente cada 20 minutos para mitigar registros no presenciales.

### RF-21: Toma de Asistencia QR para Alumno
* **Descripcion:** Permitir a los tutorados escanear el codigo QR para confirmar su asistencia[cite: 3].
* **Reglas de Negocio y Criterios:**
  * Escaneo directo desde navegador mediante libreria html5-qrcode.
  * Validacion automatica de expiracion del token para registrar la sesion.

### RF-22: Registro de Historial de Asistencias
* **Descripcion:** Registrar el historial de asistencias detallando fecha, hora, materia impartida y ubicacion.
* **Reglas de Negocio y Criterios:**
  * Se almacena: Identificador del alumno, boleta, carrera, ID de sesion, materia, fecha, hora y metodo empleado (QR o Manual).

### RF-23: Visualizacion de Historial de Asistencias
* **Descripcion:** Mostrar el historial de asistencias detallando fecha, hora, materia impartida y ubicacion.
* **Reglas de Negocio y Criterios:**
  * Tutor: Consulta el historial de sus sesiones impartidas.
  * Alumno: Consulta su historial personal de sesiones acreditadas.
  * Coordinacion: Consulta el historial institucional consolidado.

---

## Modulo 6: Encuestas de Satisfaccion y Reputacion

### RF-24: Respuesta a Encuesta de Retroalimentacion
* **Descripcion:** Presentar y permitir responder una encuesta de retroalimentacion a los alumnos al finalizar una tutoria.
* **Reglas de Negocio y Criterios:**
  * Se activa tras concluir el horario de la sesion presencial.
  * Exclusiva para alumnos con asistencia previamente confirmada.
  * Evaluacion en escala de 1 a 5 estrellas con caja de comentarios opcional.

### RF-25: Consulta de Encuestas de Retroalimentacion
* **Descripcion:** Permitir a la Coordinacion y tutores visualizar los resultados e indicadores de las encuestas para identificar areas de oportunidad.
* **Reglas de Negocio y Criterios:**
  * El promedio de estrellas se calcula y publica en el perfil del tutor dentro del catalogo.

---

## Modulo 7: Notificaciones

### RF-26: Envio de Notificaciones del Sistema
* **Descripcion:** Permitir el envio de notificaciones automaticas a los usuarios sobre cambios de estatus de solicitudes, registros de asistencia y avisos informativos.
* **Reglas de Negocio y Criterios:**
  * Automatizadas por el sistema ante resoluciones de tutores, activacion de encuestas y confirmaciones.

### RF-27: Envio de Notificaciones de Tutores
* **Descripcion:** Permitir a los roles de Coordinacion y Tutor el envio de notificaciones informativas a los usuarios.

### RF-28: Recepcion y Consulta de Notificaciones
* **Descripcion:** Permitir a los usuarios visualizar la bandeja de entrada de notificaciones recibidas dentro de la plataforma.

---

## Modulo 8: Metricas y Exportacion de Reportes

### RF-29: Generacion de Metricas y Reportes
* **Descripcion:** Permitir a Coordinacion generar reportes estadisticos.
* **Reglas de Negocio y Criterios:**
  * Incluye: Total de asistencias presenciales confirmadas, indice de atencion por maestro o alumno tutor y materias con mayor demanda en UPIICSA.

### RF-30: Exportacion de Reportes
* **Descripcion:** Permitir a la Coordinacion descargar las metricas y reportes generados en PDF.
* **Reglas de Negocio y Criterios:**
  * Procesamiento y renderizado del lado del servidor utilizando el motor QuestPDF.

