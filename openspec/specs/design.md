# 📐 Diseño Técnico del Sistema de Tutorías

Este documento consolida las decisiones de arquitectura técnica y referencias a especificaciones clave para la implementación en ASP.NET Core MVC con Entity Framework Core.

---

## 📌 1. Documentos de Referencia

1. **Requisitos Funcionales y Reglas de Negocio:**
   * [`openspec/specs/requirements.md`](file:///home/diana/MEGA/Workspace/DOTNET/ProyectoTutorias/tutorias/openspec/specs/requirements.md)
   * [`openspec/specs/REQUISITOS_Y_FLUJO_SISTEMA.md`](file:///home/diana/MEGA/Workspace/DOTNET/ProyectoTutorias/tutorias/openspec/specs/REQUISITOS_Y_FLUJO_SISTEMA.md)
2. **Arquitectura y Stack Tecnológico:**
   * [`openspec/specs/ARCHITECTURE.md`](file:///home/diana/MEGA/Workspace/DOTNET/ProyectoTutorias/tutorias/openspec/specs/ARCHITECTURE.md)
3. **Especificación del Modelo de Datos:**
   * [`openspec/specs/data-model.md`](file:///home/diana/MEGA/Workspace/DOTNET/ProyectoTutorias/tutorias/openspec/specs/data-model.md)

---

## 🛠️ 2. Directrices de Diseño para Entidades y Datos

* **Controladores Delgados (< 200 líneas):** Toda la orquestación de datos reside en servicios dedicados (`Services/`), nunca directamente con `ApplicationDbContext` en los controladores.
* **Separación de Capas:** No exponer entidades de EF Core directamente en las vistas Razor; utilizar ViewModels y DTOs en `Models/ViewModels/`.
* **Identidad y Auditoría:** Basado en `IdentityUser` con bajas lógicas (`Activo = false`, `FechaBaja`, `MotivoBaja`).
* **Relaciones Críticas:**
  * `TutorPerfil` a `Materia`: Relación Muchos a Muchos mediante `TutorMateria`.
  * `SesionTutoria` y Códigos QR: Generación dinámica con tokens de expiración de 20 minutos (`SesionQrToken`).
  * `Asistencia` y `Encuestas`: Asociación directa 1:1 de encuesta por registro de asistencia para asegurar que solo quien asistió pueda evaluar al tutor.
