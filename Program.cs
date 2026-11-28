using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using tutorias.Data;
using tutorias.Helpers;
using tutorias.Models;

// Cargar variables de entorno locales desde .env si existe
if (File.Exists(".env"))
{
    foreach (var line in File.ReadAllLines(".env"))
    {
        var trimmed = line.Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#")) continue;
        var parts = trimmed.Split('=', 2);
        if (parts.Length == 2)
        {
            var key = parts[0].Trim();
            var val = parts[1].Trim().Trim('"', '\'');
            Environment.SetEnvironmentVariable(key, val);
        }
    }
}

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Reloj simulado para pruebas de horarios (solo Development)
var resultadoSimulacion = TimeHelper.ConfigurarSimulacion(
    builder.Configuration["FECHA_SIMULADA_DEV"],
    builder.Environment.IsDevelopment());

// Configuración de DbContext con SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configuración de ASP.NET Core Identity con Roles
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddErrorDescriber<IdentityErrorDescriberEs>()
.AddDefaultTokenProviders();

// Validación inmediata de SecurityStamp para revocación de roles en sesiones activas
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

// El enlace de recuperación de contraseña vence a los 5 minutos
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(5);
});

// Configuración de cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";

    // La sesión se cierra tras 30 minutos de inactividad
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;

    // Con "Recordarme" la sesión dura 14 días sin actividad
    options.Events.OnSigningIn = context =>
    {
        if (context.Properties.IsPersistent)
        {
            context.Properties.ExpiresUtc = context.Properties.IssuedUtc?.AddDays(14);
        }
        return Task.CompletedTask;
    };
});

// Registro condicional del servicio de correo (Azure en producción / Dev simulador en local)
var azureConnString = builder.Configuration["AZURE_COMMUNICATION_SERVICES_CONNECTION_STRING"]
    ?? Environment.GetEnvironmentVariable("AZURE_COMMUNICATION_SERVICES_CONNECTION_STRING");
var azureSender = builder.Configuration["EMAIL_SENDER_ADDRESS"]
    ?? Environment.GetEnvironmentVariable("EMAIL_SENDER_ADDRESS");

if (!string.IsNullOrWhiteSpace(azureConnString) && !string.IsNullOrWhiteSpace(azureSender))
{
    builder.Services.AddScoped<tutorias.Services.IEmailSenderService, tutorias.Services.AzureEmailService>();
}
else
{
    builder.Services.AddScoped<tutorias.Services.IEmailSenderService, tutorias.Services.DevEmailService>();
}

// Add services to the container.
builder.Services.AddScoped<tutorias.Services.IUserProfileService, tutorias.Services.UserProfileService>();
builder.Services.AddScoped<tutorias.Services.ICoordinacionService, tutorias.Services.CoordinacionService>();
builder.Services.AddScoped<tutorias.Services.ITutorSolicitudService, tutorias.Services.TutorSolicitudService>();
builder.Services.AddScoped<tutorias.Services.ITutorPerfilService, tutorias.Services.TutorPerfilService>();
builder.Services.AddScoped<tutorias.Services.ICatalogoService, tutorias.Services.CatalogoService>();
builder.Services.AddScoped<tutorias.Services.IAsistenciaService, tutorias.Services.AsistenciaService>();
builder.Services.AddScoped<tutorias.Services.IQrCodeService, tutorias.Services.QrCodeService>();
builder.Services.AddScoped<tutorias.Services.IReportesService, tutorias.Services.ReportesService>();
builder.Services.AddScoped<tutorias.Services.IReportesMetricasService, tutorias.Services.ReportesMetricasService>();
builder.Services.AddScoped<tutorias.Services.IQuestPdfService, tutorias.Services.QuestPdfService>();
builder.Services.AddScoped<tutorias.Services.IEncuestasService, tutorias.Services.EncuestasService>();
builder.Services.AddScoped<tutorias.Services.INotificacionService, tutorias.Services.NotificacionService>();
builder.Services.AddScoped<tutorias.Filters.ActiveUserFilter>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<tutorias.Filters.ActiveUserFilter>();
    options.UsarMensajesEnEspanol();
});

var app = builder.Build();

switch (resultadoSimulacion)
{
    case ResultadoSimulacionHora.Activa:
        app.Logger.LogWarning("Hora simulada ACTIVA. Hora actual de la aplicación (México): {HoraSimulada:yyyy-MM-dd HH:mm:ss}", TimeHelper.NowMexico);
        break;
    case ResultadoSimulacionHora.IgnoradaFueraDeDesarrollo:
        app.Logger.LogWarning("FECHA_SIMULADA_DEV se ignoró porque el entorno no es Development. Se usa la hora real.");
        break;
    case ResultadoSimulacionHora.FormatoInvalido:
        app.Logger.LogWarning("FECHA_SIMULADA_DEV no tiene el formato yyyy-MM-ddTHH:mm:ss. Se usa la hora real.");
        break;
}

// Ejecutar Seeding de la Base de Datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbInitializer.SeedAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Páginas propias para los errores sin contenido (400, 404, etc.)
app.UseStatusCodePagesWithReExecute("/Home/Estado/{0}");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
