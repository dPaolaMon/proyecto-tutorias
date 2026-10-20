using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using tutorias.Data;
using tutorias.Models;
using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public class TutorPerfilService : ITutorPerfilService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TutorPerfilService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<TutorPerfilViewModel> GetPerfilAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.Carrera)
                .Include(u => u.PerfilTutor!)
                    .ThenInclude(p => p.MateriasImpartidas)
                        .ThenInclude(m => m.Materia)
                            .ThenInclude(mat => mat!.Carrera)
                .Include(u => u.PerfilTutor!)
                    .ThenInclude(p => p.Horarios)
                        .ThenInclude(h => h.Materia)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new InvalidOperationException("Usuario no encontrado.");
            }

            var esDocente = user.Email != null && user.Email.EndsWith("@ipn.mx") && !user.Email.EndsWith("@alumno.ipn.mx");
            var programaOAcademia = esDocente
                ? (!string.IsNullOrWhiteSpace(user.Academia) ? user.Academia : "Sin academia asignada")
                : (user.Carrera?.Nombre ?? "Sin carrera asignada");

            var modalidadTitulo = user.TipoTutor switch
            {
                TipoTutor.AlumnoTutor => "Alumno Tutor — Tutoría entre Pares",
                TipoTutor.DocenteTutor => "Docente Tutor — Tutoría Individual",
                _ => "Tutor Académico"
            };

            // Asegurar que existe PerfilTutor
            var perfil = user.PerfilTutor;
            if (perfil == null)
            {
                perfil = new TutorPerfil
                {
                    UserId = user.Id,
                    Biografia = null,
                    EsPublico = false,
                    ReputacionPromedio = 0.0
                };
                _context.TutoresPerfiles.Add(perfil);
                await _context.SaveChangesAsync();
            }

            // Cargar materias disponibles
            var queryMaterias = _context.Materias.Include(m => m.Carrera).AsQueryable();
            if (!esDocente && user.CarreraId.HasValue)
            {
                queryMaterias = queryMaterias.Where(m => m.CarreraId == user.CarreraId.Value);
            }

            var materiasDisponibles = await queryMaterias
                .OrderBy(m => m.Semestre)
                .ThenBy(m => m.Nombre)
                .Select(m => new MateriaItemViewModel
                {
                    Id = m.Id,
                    Clave = m.Clave,
                    Nombre = m.Nombre,
                    Semestre = m.Semestre,
                    CarreraNombre = m.Carrera != null ? m.Carrera.Nombre : string.Empty
                })
                .ToListAsync();

            var materiasAsignadas = perfil.MateriasImpartidas
                .Where(tm => tm.Materia != null)
                .OrderBy(tm => tm.Materia!.Semestre)
                .ThenBy(tm => tm.Materia!.Nombre)
                .Select(tm => new MateriaItemViewModel
                {
                    Id = tm.Materia!.Id,
                    Clave = tm.Materia!.Clave,
                    Nombre = tm.Materia!.Nombre,
                    Semestre = tm.Materia!.Semestre,
                    CarreraNombre = tm.Materia!.Carrera != null ? tm.Materia!.Carrera.Nombre : string.Empty
                })
                .ToList();

            var horarios = perfil.Horarios
                .Where(h => h.Activo)
                .OrderBy(h => h.DiaSemana)
                .ThenBy(h => h.HoraInicio)
                .Select(h => new TutorHorarioItemViewModel
                {
                    Id = h.Id,
                    DiaSemana = h.DiaSemana,
                    HoraInicio = h.HoraInicio,
                    HoraFin = h.HoraFin,
                    Lugar = h.Lugar,
                    MateriaId = h.MateriaId,
                    MateriaNombre = h.Materia != null ? h.Materia.Nombre : "Tutoría General",
                    Activo = h.Activo
                })
                .ToList();

            return new TutorPerfilViewModel
            {
                PerfilId = perfil.Id,
                UserId = user.Id,
                NombreCompleto = user.NombreCompleto,
                Boleta = user.Boleta,
                Email = user.Email ?? string.Empty,
                FotoUrl = user.FotoUrl,
                EsDocente = esDocente,
                ProgramaOAcademia = programaOAcademia,
                ModalidadTitulo = modalidadTitulo,
                Biografia = perfil.Biografia,
                EsPublico = perfil.EsPublico,
                ReputacionPromedio = perfil.ReputacionPromedio,
                MateriasSeleccionadasIds = materiasAsignadas.Select(m => m.Id).ToList(),
                MateriasDisponibles = materiasDisponibles,
                MateriasAsignadas = materiasAsignadas,
                Horarios = horarios
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> ActualizarPerfilAsync(string userId, GuardarTutorPerfilInputModel model)
        {
            var perfil = await _context.TutoresPerfiles
                .Include(p => p.MateriasImpartidas)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (perfil == null)
            {
                return (false, "Perfil de tutor no encontrado.");
            }

            perfil.Biografia = model.Biografia?.Trim();
            perfil.EsPublico = model.EsPublico;

            // Actualizar materias impartidas
            var seleccionadas = model.MateriasSeleccionadasIds ?? new List<int>();

            // Remover materias desmarcadas
            var aEliminar = perfil.MateriasImpartidas
                .Where(tm => !seleccionadas.Contains(tm.MateriaId))
                .ToList();

            foreach (var item in aEliminar)
            {
                _context.TutoresMaterias.Remove(item);
            }

            // Agregar materias nuevas
            var idsActuales = perfil.MateriasImpartidas.Select(tm => tm.MateriaId).ToHashSet();
            foreach (var matId in seleccionadas)
            {
                if (!idsActuales.Contains(matId))
                {
                    _context.TutoresMaterias.Add(new TutorMateria
                    {
                        TutorPerfilId = perfil.Id,
                        MateriaId = matId,
                        FechaAsignacion = DateTime.UtcNow
                    });
                }
            }

            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> GuardarHorarioAsync(string userId, GuardarHorarioInputModel model)
        {
            var perfil = await _context.TutoresPerfiles
                .Include(p => p.Horarios)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (perfil == null)
            {
                return (false, "Perfil de tutor no encontrado.");
            }

            // Validaciones de día y rango
            if (model.DiaSemana < DiaSemana.Lunes || model.DiaSemana > DiaSemana.Viernes)
            {
                return (false, "Las tutorías solo pueden programarse de Lunes a Viernes.");
            }

            var horaMin = TimeSpan.FromHours(7);
            var horaMax = TimeSpan.FromHours(22);

            if (model.HoraInicio < horaMin || model.HoraFin > horaMax)
            {
                return (false, "El horario de atención debe ubicarse dentro del rango de 07:00 a 22:00.");
            }

            if (model.HoraInicio >= model.HoraFin)
            {
                return (false, "La hora de inicio debe ser anterior a la hora de fin.");
            }

            if (string.IsNullOrWhiteSpace(model.Lugar))
            {
                return (false, "Debes especificar el salón o lugar físico asignado.");
            }

            // Validar no solapamiento para el mismo tutor
            var solapado = perfil.Horarios.Any(h =>
                h.Activo &&
                h.DiaSemana == model.DiaSemana &&
                h.Id != (model.Id ?? 0) &&
                h.HoraInicio < model.HoraFin &&
                h.HoraFin > model.HoraInicio);

            if (solapado)
            {
                return (false, $"Ya cuentas con un bloque de tutoría asignado en ese rango de horario el día {model.DiaSemana}.");
            }

            if (model.Id.HasValue && model.Id.Value > 0)
            {
                var horarioExistente = perfil.Horarios.FirstOrDefault(h => h.Id == model.Id.Value);
                if (horarioExistente == null)
                {
                    return (false, "El bloque de horario no fue encontrado.");
                }

                horarioExistente.DiaSemana = model.DiaSemana;
                horarioExistente.HoraInicio = model.HoraInicio;
                horarioExistente.HoraFin = model.HoraFin;
                horarioExistente.Lugar = model.Lugar.Trim();
                horarioExistente.MateriaId = model.MateriaId;
            }
            else
            {
                var nuevoHorario = new TutorHorario
                {
                    TutorPerfilId = perfil.Id,
                    DiaSemana = model.DiaSemana,
                    HoraInicio = model.HoraInicio,
                    HoraFin = model.HoraFin,
                    Lugar = model.Lugar.Trim(),
                    MateriaId = model.MateriaId,
                    Activo = true
                };
                _context.TutoresHorarios.Add(nuevoHorario);
            }

            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> EliminarHorarioAsync(string userId, int horarioId)
        {
            var perfil = await _context.TutoresPerfiles
                .Include(p => p.Horarios)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (perfil == null)
            {
                return (false, "Perfil de tutor no encontrado.");
            }

            var horario = perfil.Horarios.FirstOrDefault(h => h.Id == horarioId);
            if (horario == null)
            {
                return (false, "El horario no fue encontrado o no pertenece a tu perfil.");
            }

            _context.TutoresHorarios.Remove(horario);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> CambiarVisibilidadAsync(string userId, bool esPublico)
        {
            var perfil = await _context.TutoresPerfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (perfil == null)
            {
                return (false, "Perfil de tutor no encontrado.");
            }

            perfil.EsPublico = esPublico;
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> EliminarPerfilAcademicoAsync(string userId)
        {
            var perfil = await _context.TutoresPerfiles
                .Include(p => p.MateriasImpartidas)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (perfil == null)
            {
                return (false, "Perfil de tutor no encontrado.");
            }

            if (perfil.MateriasImpartidas.Any())
            {
                _context.TutoresMaterias.RemoveRange(perfil.MateriasImpartidas);
            }

            perfil.Biografia = null;
            perfil.EsPublico = false;

            await _context.SaveChangesAsync();
            return (true, null);
        }
    }
}
