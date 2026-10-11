using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using tutorias.Models;

namespace tutorias.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Asegurar que la base de datos existe/está migrada
            await context.Database.MigrateAsync();

            // 1. Seeding de Roles
            string[] roles = new[] { "Alumno", "Docente", "Tutor", "Coordinador" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seeding de Catálogo de Carreras y Materias
            if (!await context.Carreras.AnyAsync())
            {
                var carreraInfo = new Carrera
                {
                    Nombre = "Ingeniería en Informática",
                    Clave = "II",
                    Materias = new List<Materia>
                    {
                        new Materia { Nombre = "Cálculo Diferencial e Integral", Clave = "CALC01" },
                        new Materia { Nombre = "Estructura de Datos", Clave = "ED01" },
                        new Materia { Nombre = "Bases de Datos", Clave = "BD01" },
                        new Materia { Nombre = "Programación Orientada a Objetos", Clave = "POO01" }
                    }
                };

                var carreraInd = new Carrera
                {
                    Nombre = "Ingeniería Industrial",
                    Clave = "IND",
                    Materias = new List<Materia>
                    {
                        new Materia { Nombre = "Estadística Aplicada", Clave = "EST01" },
                        new Materia { Nombre = "Investigación de Operaciones", Clave = "INV01" }
                    }
                };

                context.Carreras.AddRange(carreraInfo, carreraInd);
                await context.SaveChangesAsync();
            }

            var carreraPrincipal = await context.Carreras.FirstOrDefaultAsync();

            // 3. Usuario Coordinador de Prueba
            var coordEmail = "coordinacion@ipn.mx";
            if (await userManager.FindByEmailAsync(coordEmail) == null)
            {
                var coordUser = new ApplicationUser
                {
                    UserName = coordEmail,
                    Email = coordEmail,
                    EmailConfirmed = true,
                    NombreCompleto = "Coordinador de Tutorías IPN",
                    Boleta = "EMP990011",
                    CarreraId = carreraPrincipal?.Id,
                    Semestre = 9,
                    PerfilCompleto = true
                };

                var result = await userManager.CreateAsync(coordUser, "CoordTutorias#2026");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(coordUser, "Coordinador");
                }
            }

            // 4. Usuario Tutor de Prueba
            var tutorEmail = "tutor.demo@ipn.mx";
            if (await userManager.FindByEmailAsync(tutorEmail) == null)
            {
                var tutorUser = new ApplicationUser
                {
                    UserName = tutorEmail,
                    Email = tutorEmail,
                    EmailConfirmed = true,
                    NombreCompleto = "Mtro. Tutor Demostración",
                    Boleta = "EMP881122",
                    CarreraId = carreraPrincipal?.Id,
                    Semestre = 8,
                    TipoTutor = TipoTutor.DocenteTutor,
                    PerfilCompleto = true
                };

                var result = await userManager.CreateAsync(tutorUser, "TutorDemo#2026");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(tutorUser, "Tutor");

                    var perfil = new TutorPerfil
                    {
                        UserId = tutorUser.Id,
                        Biografia = "Docente especialista en Algoritmos y Programación en C# / .NET.",
                        EsPublico = true,
                        ReputacionPromedio = 4.8,
                        SalonAsignado = "Edificio 2, Salón 204"
                    };
                    context.TutoresPerfiles.Add(perfil);
                    await context.SaveChangesAsync();
                }
            }

            // 5. Usuario Alumno de Prueba
            var alumnoEmail = "alumno.demo@alumno.ipn.mx";
            if (await userManager.FindByEmailAsync(alumnoEmail) == null)
            {
                var alumnoUser = new ApplicationUser
                {
                    UserName = alumnoEmail,
                    Email = alumnoEmail,
                    EmailConfirmed = true,
                    NombreCompleto = "Alumno Prueba Politécnico",
                    Boleta = "2024600123",
                    CarreraId = carreraPrincipal?.Id,
                    Semestre = 4,
                    PerfilCompleto = true
                };

                var result = await userManager.CreateAsync(alumnoUser, "AlumnoDemo#2026");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(alumnoUser, "Alumno");
                }
            }
        }
    }
}
