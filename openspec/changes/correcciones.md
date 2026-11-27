# Correcciones pendientes — pruebas de caja negra

Fecha: 2026-11-26 · Rama: `test/correcciones-pruebas` · Estado: **sin iniciar**

## Contexto
- Fuentes: `openspec/changes/Pruebas.md`, `openspec/changes/Pruebas2_0.md` y el plan `plan_correccion_errores_pruebas.md` (HAL-001 a HAL-009).
- HAL-001 a HAL-009 ya tienen cambios **sin commit** en la rama. El `walkthrough.md` de ese trabajo reporta compilación sin errores.
- Lo que sigue sale de leer el código. No se compiló ni se ejecutó la app.

## 1. Huecos en las correcciones HAL

- [ ] **HAL-007 (0 estrellas):** `Controllers/AsistenciaController.cs:147` todavía rechaza `calificacion < 1`, así que calificar con 0 falla. Cambiar a `< 0`. Revisar también `[Range(1, 5)]` en `Models/Asistencia.cs:24` (si cambia el modelo, ver si requiere migración) y que el panel de encuestas no tiene conteo de 0 estrellas (`Services/EncuestasService.cs`).
- [ ] **HAL-001 (contraseña 8–20):** `asp-for` con `StringLength(20)` normalmente genera `maxlength="20"`, que recortaría 21 caracteres y dejaría entrar (CP-02) o registrar (CP-01). Verificar el HTML de `Login`, `Register` y `ResetPassword`. Considerar además que cuentas con contraseñas de menos de 8 o más de 20 ya no podrán iniciar sesión.
- [ ] **HAL-009 (PDF sin registros):** el aviso saldría duplicado: lo muestran `Views/Shared/_Layout.cshtml` y `Views/Reportes/Metricas.cshtml`.
- [ ] **HAL-003 (1 a 5 materias):** el cambio de visibilidad (`ToggleVisibilidad` / `CambiarVisibilidadAsync`) no valida materias y permite publicar con 0.
- [ ] **HAL-004 (eliminar perfil):** el botón no se deshabilita cuando no hay perfil (CP-10). La causa original de "no permite eliminar" no se identificó: reproducir.
- [ ] **HAL-005 (traslape 09:00–10:01):** la condición anterior era equivalente a la nueva, así que el fallo reportado sigue sin explicación: reproducir.
- [ ] **HAL-002, HAL-006, HAL-008:** probar en la app. Nota de HAL-002: `ValidationInterval = TimeSpan.Zero` consulta la base de datos en cada petición autenticada; medir con JMeter.

## 2. Requisitos adicionales aceptados

- [ ] **Código de recuperación de contraseña válido 5 minutos** (CP-04). Hoy no se configura la vigencia.
- [ ] **Sesión que expira a los 30 minutos** (CP-03). Hoy no se configura. El rango está marcado como "supuesto" en las pruebas: confirmar si es por inactividad o tiempo total.

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

## 4. Pendiente de decisión — NO implementar

- [ ] **QR válido 3 minutos en lugar de 20** (CP-18). Consultar con quien lleva el análisis.
  - Cambios: `Services/AsistenciaService.cs` (líneas 308, 344 y mensaje 381), `Controllers/AsistenciaController.cs:86`, `Views/Asistencia/GenerarQr.cshtml` (líneas 67 y 155). Conviene una sola constante. No requiere migración.
  - Impacto: menos margen para alumnos que deben iniciar sesión tras escanear; el código se renueva unas 20 veces por hora; revisar planes de JMeter y documentos que dicen 20 minutos.

## Verificación al implementar
- `dotnet build` sin advertencias ni errores; `dotnet ef database update` si cambia el modelo.
- Repetir en la app cada caso CP afectado, incluidos los valores límite (0 estrellas, 21 caracteres, 0 materias, 09:00–10:01, PDF sin registros).
- Provocar cada mensaje de la sección 3 y confirmar que sale en español.
