using System.Text.Json;
using System.Text.Json.Serialization;
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

            // 2. Seeding de Catálogo UPIICSA (Carreras, Materias y Equivalencias)
            await SeedUpiicsaCatalogAsync(context);

            var carreraInformatica = await context.Carreras.FirstOrDefaultAsync(c => c.Clave == "N")
                ?? await context.Carreras.FirstOrDefaultAsync();

            // 3. Usuario Coordinador de Prueba
            var coordEmail = "coordinacion@ipn.mx";
            var coordUser = await userManager.FindByEmailAsync(coordEmail);
            if (coordUser == null)
            {
                coordUser = new ApplicationUser
                {
                    UserName = coordEmail,
                    Email = coordEmail,
                    EmailConfirmed = true,
                    Nombre = "Coordinador",
                    ApellidoPaterno = "Tutorías",
                    ApellidoMaterno = "IPN",
                    Boleta = "9900112233",
                    Academia = "Academia de Informática",
                    PerfilCompleto = true,
                    Activo = true
                };

                var result = await userManager.CreateAsync(coordUser, "CoordTutorias#2026");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(coordUser, "Coordinador");
                }
            }
            else if (string.IsNullOrWhiteSpace(coordUser.Nombre))
            {
                coordUser.Nombre = "Coordinador";
                coordUser.ApellidoPaterno = "Tutorías";
                coordUser.ApellidoMaterno = "IPN";
                await userManager.UpdateAsync(coordUser);
            }

            // 4. Usuario Tutor de Prueba (Docente)
            var tutorEmail = "tutor.demo@ipn.mx";
            var tutorUser = await userManager.FindByEmailAsync(tutorEmail);
            if (tutorUser == null)
            {
                tutorUser = new ApplicationUser
                {
                    UserName = tutorEmail,
                    Email = tutorEmail,
                    EmailConfirmed = true,
                    Nombre = "Demostración",
                    ApellidoPaterno = "Tutor",
                    ApellidoMaterno = "Docente",
                    Boleta = "8811223344",
                    Academia = "Academia de Ciencias Básicas",
                    TipoTutor = TipoTutor.DocenteTutor,
                    PerfilCompleto = true,
                    Activo = true
                };

                var result = await userManager.CreateAsync(tutorUser, "TutorDemo#2026");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(tutorUser, "Tutor");
                    await userManager.AddToRoleAsync(tutorUser, "Docente");

                    var perfil = new TutorPerfil
                    {
                        UserId = tutorUser.Id,
                        Biografia = "Docente especialista en Algoritmos y Programación en C# / .NET en UPIICSA.",
                        EsPublico = true,
                        ReputacionPromedio = 4.8,
                        SalonAsignado = "Edificio 2, Salón 204"
                    };
                    context.TutoresPerfiles.Add(perfil);
                    await context.SaveChangesAsync();
                }
            }
            else if (string.IsNullOrWhiteSpace(tutorUser.Nombre))
            {
                tutorUser.Nombre = "Demostración";
                tutorUser.ApellidoPaterno = "Tutor";
                tutorUser.ApellidoMaterno = "Docente";
                await userManager.UpdateAsync(tutorUser);
            }

            // 5. Usuario Alumno de Prueba
            var alumnoEmail = "alumno.demo@alumno.ipn.mx";
            var alumnoUser = await userManager.FindByEmailAsync(alumnoEmail);
            if (alumnoUser == null)
            {
                alumnoUser = new ApplicationUser
                {
                    UserName = alumnoEmail,
                    Email = alumnoEmail,
                    EmailConfirmed = true,
                    Nombre = "Prueba",
                    ApellidoPaterno = "Politécnico",
                    ApellidoMaterno = "Alumno",
                    Boleta = "2024600123",
                    CarreraId = carreraInformatica?.Id,
                    Semestre = 4,
                    PerfilCompleto = true,
                    Activo = true
                };

                var result = await userManager.CreateAsync(alumnoUser, "AlumnoDemo#2026");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(alumnoUser, "Alumno");
                }
            }
            else if (string.IsNullOrWhiteSpace(alumnoUser.Nombre))
            {
                alumnoUser.Nombre = "Prueba";
                alumnoUser.ApellidoPaterno = "Politécnico";
                alumnoUser.ApellidoMaterno = "Alumno";
                await userManager.UpdateAsync(alumnoUser);
            }
        }

        private static async Task SeedUpiicsaCatalogAsync(ApplicationDbContext context)
        {
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "Seeds", "upiicsa_catalogo.json");
            if (!File.Exists(jsonPath))
            {
                jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Seeds", "upiicsa_catalogo.json");
            }

            if (!File.Exists(jsonPath)) return;

            var jsonText = await File.ReadAllTextAsync(jsonPath);
            var catalog = JsonSerializer.Deserialize<CatalogoSeedDto>(jsonText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (catalog == null || catalog.Carreras.Count == 0) return;

            // 1. Cargar o actualizar carreras y materias
            foreach (var cDto in catalog.Carreras)
            {
                var carrera = await context.Carreras
                    .Include(c => c.Materias)
                    .FirstOrDefaultAsync(c => c.Clave == cDto.Clave || c.Nombre == cDto.Nombre);

                if (carrera == null)
                {
                    carrera = new Carrera
                    {
                        Nombre = cDto.Nombre,
                        Clave = cDto.Clave
                    };
                    context.Carreras.Add(carrera);
                    await context.SaveChangesAsync();
                }
                else
                {
                    carrera.Nombre = cDto.Nombre;
                    carrera.Clave = cDto.Clave;
                }

                // Cargar materias de esta carrera
                var materiasExistentes = carrera.Materias.ToDictionary(m => m.Clave);
                foreach (var mDto in cDto.Materias)
                {
                    if (materiasExistentes.TryGetValue(mDto.Clave, out var materiaExistente))
                    {
                        materiaExistente.Nombre = mDto.Nombre;
                        materiaExistente.Semestre = mDto.Semestre;
                        materiaExistente.CodigoEquivalencia = mDto.CodigoEquivalencia;
                    }
                    else
                    {
                        carrera.Materias.Add(new Materia
                        {
                            Nombre = mDto.Nombre,
                            Clave = mDto.Clave,
                            Semestre = mDto.Semestre,
                            CodigoEquivalencia = mDto.CodigoEquivalencia,
                            CarreraId = carrera.Id
                        });
                    }
                }

                await context.SaveChangesAsync();
            }

            // 2. Cargar tabla de equivalencias explícitas
            if (catalog.Equivalencias.Count > 0)
            {
                var todasMaterias = await context.Materias
                    .GroupBy(m => m.Clave)
                    .ToDictionaryAsync(g => g.Key, g => g.First());

                foreach (var eqDto in catalog.Equivalencias)
                {
                    if (todasMaterias.TryGetValue(eqDto.ClaveOrigen, out var mOrigen) &&
                        todasMaterias.TryGetValue(eqDto.ClaveEquivalente, out var mDestino))
                    {
                        var yaExiste = await context.MateriasEquivalencias.AnyAsync(e =>
                            e.MateriaOrigenId == mOrigen.Id && e.MateriaEquivalenteId == mDestino.Id);

                        if (!yaExiste)
                        {
                            context.MateriasEquivalencias.Add(new MateriaEquivalencia
                            {
                                MateriaOrigenId = mOrigen.Id,
                                MateriaEquivalenteId = mDestino.Id,
                                EsBidireccional = eqDto.Bidireccional
                            });
                        }

                        if (eqDto.Bidireccional)
                        {
                            var yaExisteInverso = await context.MateriasEquivalencias.AnyAsync(e =>
                                e.MateriaOrigenId == mDestino.Id && e.MateriaEquivalenteId == mOrigen.Id);

                            if (!yaExisteInverso)
                            {
                                context.MateriasEquivalencias.Add(new MateriaEquivalencia
                                {
                                    MateriaOrigenId = mDestino.Id,
                                    MateriaEquivalenteId = mOrigen.Id,
                                    EsBidireccional = true
                                });
                            }
                        }
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        private class CatalogoSeedDto
        {
            public string? UnidadAcademica { get; set; }
            public List<CarreraSeedDto> Carreras { get; set; } = new();
            public List<EquivalenciaSeedDto> Equivalencias { get; set; } = new();
        }

        private class CarreraSeedDto
        {
            public string Nombre { get; set; } = string.Empty;
            public string Clave { get; set; } = string.Empty;
            public List<MateriaSeedDto> Materias { get; set; } = new();
        }

        private class MateriaSeedDto
        {
            public string Nombre { get; set; } = string.Empty;
            public string Clave { get; set; } = string.Empty;
            public int? Semestre { get; set; }
            public string? CodigoEquivalencia { get; set; }
        }

        private class EquivalenciaSeedDto
        {
            [JsonPropertyName("clave_origen")]
            public string ClaveOrigen { get; set; } = string.Empty;

            [JsonPropertyName("materia_origen")]
            public string MateriaOrigen { get; set; } = string.Empty;

            [JsonPropertyName("carrera_origen")]
            public string CarreraOrigen { get; set; } = string.Empty;

            [JsonPropertyName("clave_equivalente")]
            public string ClaveEquivalente { get; set; } = string.Empty;

            [JsonPropertyName("materia_equivalente")]
            public string MateriaEquivalente { get; set; } = string.Empty;

            [JsonPropertyName("carrera_equivalente")]
            public string CarreraEquivalente { get; set; } = string.Empty;

            [JsonPropertyName("bidireccional")]
            public bool Bidireccional { get; set; } = true;
        }
    }
}
