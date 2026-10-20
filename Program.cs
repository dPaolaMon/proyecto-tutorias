using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using tutorias.Data;
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

var builder = WebApplication.CreateBuilder(args);

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
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configuración de cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
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
builder.Services.AddScoped<tutorias.Services.IEncuestasService, tutorias.Services.EncuestasService>();
builder.Services.AddScoped<tutorias.Services.INotificacionService, tutorias.Services.NotificacionService>();
builder.Services.AddScoped<tutorias.Filters.ActiveUserFilter>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<tutorias.Filters.ActiveUserFilter>();
});

var app = builder.Build();

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

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
