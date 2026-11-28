# Correcciones pendientes — pruebas de caja negra

Fecha: 2026-11-26 · Rama: `test/correcciones-pruebas` · Estado: **sección 1 en commit `cf40e41`; sección 2 hecha, sin commit**

## Contexto
- Fuentes: `openspec/changes/Pruebas.md`, `openspec/changes/Pruebas2_0.md` y el plan `plan_correccion_errores_pruebas.md` (HAL-001 a HAL-009).
- HAL-001 a HAL-009 ya tienen cambios **sin commit** en la rama. El `walkthrough.md` de ese trabajo reporta compilación sin errores.
- Lo que sigue sale de leer el código. No se compiló ni se ejecutó la app.

## 1. Huecos en las correcciones HAL

- [x] **HAL-007 (0 estrellas):** `Controllers/AsistenciaController.cs:147` todavía rechaza `calificacion < 1`, así que calificar con 0 falla. Cambiar a `< 0`. Revisar también `[Range(1, 5)]` en `Models/Asistencia.cs:24` (si cambia el modelo, ver si requiere migración) y que el panel de encuestas no tiene conteo de 0 estrellas (`Services/EncuestasService.cs`).
  - Hecho y probado: calificar con 0 funciona y el panel lo cuenta. Sin migración.
- [x] **HAL-001 (contraseña 8–20):** `asp-for` con `StringLength(20)` normalmente genera `maxlength="20"`, que recortaría 21 caracteres y dejaría entrar (CP-02) o registrar (CP-01). Verificar el HTML de `Login`, `Register` y `ResetPassword`. Considerar además que cuentas con contraseñas de menos de 8 o más de 20 ya no podrán iniciar sesión.
  - Hecho y probado en Login y Registro (7 y 21 rechazados, 8 y 20 aceptados en registro). En ResetPassword solo se revisó el HTML. Las cuentas fuera de 8–20 no pueden iniciar sesión: aceptado.
- [x] **HAL-009 (PDF sin registros):** el aviso saldría duplicado: lo muestran `Views/Shared/_Layout.cshtml` y `Views/Reportes/Metricas.cshtml`.
  - Hecho y probado: sale un solo aviso.
- [x] **HAL-003 (1 a 5 materias):** el cambio de visibilidad (`ToggleVisibilidad` / `CambiarVisibilidadAsync`) no valida materias y permite publicar con 0.
  - Hecho y probado con 0 y 1 materia (el límite de 6 al publicar no se probó). Los perfiles que ya estaban publicados sin materias siguen publicados.
- [x] **HAL-004 (eliminar perfil):** el botón no se deshabilita cuando no hay perfil (CP-10). La causa original de "no permite eliminar" no se identificó: reproducir.
  - Botón deshabilitado sin perfil y eliminación probada. El fallo original no se reprodujo.
- [x] **HAL-005 (traslape 09:00–10:01):** la condición anterior era equivalente a la nueva, así que el fallo reportado sigue sin explicación: reproducir.
  - No se reproduce: 09:00–10:01 se rechaza al crear y al editar. Sin cambios de código; causa del reporte sin identificar.
- [x] **HAL-002, HAL-006, HAL-008:** probar en la app. Nota de HAL-002: `ValidationInterval = TimeSpan.Zero` consulta la base de datos en cada petición autenticada; medir con JMeter.
  - Probados en la app. JMeter (`tests/jmeter/hal002_sesion.jmx`, local, 50 hilos sin pausa): con intervalo 0, ~295 ms y ~157 pet/s; con 30 min, ~178 ms y ~253 pet/s. Se mantiene en 0.

## 2. Requisitos adicionales aceptados

- [x] **Código de recuperación de contraseña válido 5 minutos** (CP-04). Hoy no se configura la vigencia.
  - Hecho y probado: aceptado a los 299 s y rechazado a los 301 s (aún con `Invalid token.`, ver 3.1). La app envía un enlace, no un código: ajustar la redacción de CP-04.
- [x] **Cierre de sesión tras 30 minutos de inactividad** (CP-03). **Decidido:** cuenta la inactividad, no el tiempo total. Hoy no se configura.
  - Hecho y probado: activa a los 29 min sin actividad y vencida a los 31; con actividad sigue activa a los 35. **Decidido:** con "Recordarme" la sesión dura 14 días. Se corrigió además el error 400 al pulsar "Cerrar sesión" con la sesión vencida (`Filters/SesionVencidaAlSalirAttribute.cs`).
  - En `Program.cs` (`ConfigureApplicationCookie`): `ExpireTimeSpan` de 30 minutos y `SlidingExpiration = true`.
  - Verificar el tiempo real: la renovación deslizante por sí sola solo ocurre pasada la mitad del plazo. La validación con `ValidationInterval = TimeSpan.Zero` (HAL-002) debería renovar en cada petición; si ese intervalo cambia, volver a comprobar.
  - Las pantallas que consultan al servidor solas cuentan como actividad. La del QR consulta cada 5 segundos, así que la sesión del tutor no expira mientras esté abierta. **Decidido: es aceptable**, no se cambia.
  - Probar también con "Recordarme" activado.
  - CP-03 está redactado como tiempo con la sesión abierta sin tocar nada: 31 minutos sin actividad debe pedir iniciar sesión.

## 3. Mensajes de error

### 3.1 ASP.NET Identity en inglés
No hay `IdentityErrorDescriber`. Solución: una clase con los mensajes en español, registrada en `Program.cs`.

- [ ] Correo ya registrado: `Username 'x' is already taken.` / `Email 'x' is already taken.`
- [ ] Contraseña sin número: `Passwords must have at least one digit ('0'-'9').`
- [ ] Contraseña sin minúscula: `Passwords must have at least one lowercase ('a'-'z').`
- [ ] Enlace de restablecimiento vencido o inválido: `Invalid token.`
- [ ] Cambio simultáneo: `Optimistic concurrency failure, object has been modified.`
- [ ] Roles: `User already in role 'x'.` / `User is not in role 'x'.`
- [ ] Genérico: `An unknown failure has occurred.`

### 3.2 Validaciones de formulario
- [ ] Registro, confirmar contraseña vacío: `The Confirmar contraseña field is required.`
- [ ] Restablecer, confirmar contraseña vacío: `The Confirmar nueva contraseña field is required.`
- [ ] `Views/Account/DeleteProfile.cshtml:40`, casilla sin marcar: `This field is required.`
- [ ] `Views/Account/Profile.cshtml:214`, casilla sin marcar: mensaje nativo en el idioma del navegador.
- [ ] `ResetPasswordViewModel.Token`: `[Required]` sin mensaje (campo oculto, probablemente no se ve).

### 3.3 Páginas de error
- [ ] `Views/Shared/Error.cshtml`: plantilla en inglés.
- [ ] `AccountController.ResetPassword` (GET) sin token: abre `View("Error")` sin modelo; probablemente lanza excepción.
- [ ] No hay página propia para 404 ni 400.

### 3.4 En español pero sin decir qué falló
- [ ] Guardar perfil de tutor: "Por favor verifica los campos ingresados."
- [ ] Guardar horario: "Por favor verifica los datos del horario y salón asignado."
- [ ] Inicio de sesión: "Intento de inicio de sesión no válido."
- [ ] Inicio de sesión: "Contraseña incorrecta." revela que el correo existe. Propuesta: "Correo o contraseña incorrectos" (lo que espera CP-02).

## 4. Decidido sin cambios de código

- **El QR se mantiene en 20 minutos.** No se cambia a 3.
- [ ] Actualizar CP-18 en el documento de pruebas: hoy espera 3 minutos (180 s). Con la decisión, los límites son 1199 s, 1200 s (aceptados) y 1201 s (rechazado).

## Verificación al implementar
- `dotnet build` sin advertencias ni errores; `dotnet ef database update` si cambia el modelo.
- Repetir en la app cada caso CP afectado, incluidos los valores límite (0 estrellas, 21 caracteres, 0 materias, 09:00–10:01, PDF sin registros).
- Provocar cada mensaje de la sección 3 y confirmar que sale en español.
