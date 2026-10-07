# AGENTS.md

## Dev environment tips
- El proyecto se ubica dentro del directorio `tutorias/`.
- Usa `dotnet build` para compilar el proyecto y verificar errores de sintaxis y tipos.
- Ejecuta `dotnet run` para iniciar el servidor local de desarrollo.
- Aplica las migraciones a la base de datos local con `dotnet ef database update`.
- Para crear una nueva migración tras cambiar el modelo en `Models/`, usa `dotnet ef migrations add <NombreDeLaMigracion>`.
- Las variables de entorno y secretos deben cargarse desde `.env` localmente o desde Azure App Settings en producción; nunca agregues secretos en `appsettings.json`. Si agregas una variable nueva, regístrala también en `.env.example`.
- Para la paleta visual, utiliza las variables CSS institucionales definidas en `wwwroot/css/site.css` (Guinda IPN `#750946`, Guinda oscuro `#4d0630`, Gris claro `#f8f9fa`, Gris texto `#495057`). No hardcodees valores HEX en las vistas Razor.

## Architecture & Code Guidelines
- Manten los controladores delgados en `Controllers/` (< 200 líneas). La lógica de negocio pertenece a los servicios en `Services/` (`QrCodeService`, `QuestPdfService`, `EmailService`, etc.). Los controladores no deben acceder directamente a `ApplicationDbContext`.
- Usa tipado estricto: está prohibido el uso de `dynamic` y `object` sin tipo explícito.
- No pases entidades de Entity Framework Core directamente a las vistas; utiliza ViewModels/DTOs.
- Valida `ModelState.IsValid` en todo método de acción `[HttpPost]`. Retorna mensajes claros de error, jamás expongas stack traces.
- Protege los endpoints con `[Authorize]` o `[Authorize(Roles = "...")]`. Únicamente son públicos los endpoints de Login, Registro y el Catálogo público.
- Valida siempre en el servidor la propiedad de los recursos y el rol correspondiente del usuario.
- Todas las vistas Razor deben ser responsive e implementadas con enfoque *mobile-first* usando Bootstrap 5 o Tailwind CSS.
- Todos los textos de la interfaz deben estar escritos en español.
- Utiliza ampliamente los comandos integrados de `dotnet`

## Testing instructions
- La herramienta de pruebas estandarizada para el proyecto es Apache JMeter.
- Diseña y ejecuta planes de prueba `.jmx` con Apache JMeter para validar la carga, concurrencia y tiempos de respuesta de los endpoints principales (ej. escaneo de QR, autenticación y consultas al catálogo).
- Antes de enviar cambios, verifica que la solución compile limpiamente sin advertencias ni errores mediante `dotnet build`.
- Asegúrate de haber ejecutado `dotnet ef database update` para verificar la integridad de las migraciones sin romper el esquema de la base de datos.

## PR instructions
- Formato del título del PR: `[Tutorias] <Descripción breve del cambio>`
- Asegúrate de haber ejecutado `dotnet build` localmente y de que no existan errores de compilación ni advertencias de tipos.
- Revisa que las migraciones de EF Core se hayan generado automáticamente mediante comandos CLI (`dotnet ef migrations add`), evitando editar manualmente los archivos dentro de `Migrations/`.
- Confirma que no se incluyan archivos de secretos ni credenciales en el commit (`.env` debe mantenerse dentro de `.gitignore`).
