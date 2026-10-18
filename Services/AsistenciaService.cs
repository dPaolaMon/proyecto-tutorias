using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tutorias.Data;
using tutorias.Helpers;
using tutorias.Models;
using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public class AsistenciaService : IAsistenciaService
    {
        private readonly ApplicationDbContext _context;

        public AsistenciaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BuscarAlumnoResultViewModel>> BuscarAlumnosAsync(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
            {
                return new List<BuscarAlumnoResultViewModel>();
            }

            var t = termino.Trim().ToLower();

            var alumnos = await _context.Users
                .Include(u => u.Carrera)
                .Where(u => u.Activo && (
                    u.Boleta.ToLower().Contains(t) ||
                    u.Nombre.ToLower().Contains(t) ||
                    u.ApellidoPaterno.ToLower().Contains(t) ||
                    u.ApellidoMaterno.ToLower().Contains(t) ||
                    (u.Email != null && u.Email.ToLower().Contains(t))))
                .OrderBy(u => u.Nombre)
                .ThenBy(u => u.ApellidoPaterno)
                .Take(10)
                .Select(u => new BuscarAlumnoResultViewModel
                {
                    Id = u.Id,
                    NombreCompleto = u.NombreCompleto,
                    Boleta = u.Boleta,
                    Email = u.Email ?? string.Empty,
                    CarreraNombre = u.Carrera != null ? u.Carrera.Nombre : (u.Academia ?? "UPIICSA"),
                    FotoUrl = u.FotoUrl
                })
                .ToListAsync();

            return alumnos;
        }

        public async Task<PanelTutorAsistenciaViewModel> GetPanelTutorAsync(string tutorId)
        {
            var now = TimeHelper.NowMexico;
            DiaSemana? diaActual = now.DayOfWeek switch
            {
                DayOfWeek.Monday => DiaSemana.Lunes,
                DayOfWeek.Tuesday => DiaSemana.Martes,
                DayOfWeek.Wednesday => DiaSemana.Miercoles,
                DayOfWeek.Thursday => DiaSemana.Jueves,
                DayOfWeek.Friday => DiaSemana.Viernes,
                _ => null
            };

            string diaActualNombre = now.DayOfWeek switch
            {
                DayOfWeek.Monday => "Lunes",
                DayOfWeek.Tuesday => "Martes",
                DayOfWeek.Wednesday => "Miércoles",
                DayOfWeek.Thursday => "Jueves",
                DayOfWeek.Friday => "Viernes",
                DayOfWeek.Saturday => "Sábado",
                DayOfWeek.Sunday => "Domingo",
                _ => "Desconocido"
            };

            var horaActual = now.TimeOfDay;

            var perfil = await _context.TutoresPerfiles
                .Include(p => p.Horarios.Where(h => h.Activo))
                    .ThenInclude(h => h.Materia)
                .Include(p => p.MateriasImpartidas)
                    .ThenInclude(m => m.Materia)
                .FirstOrDefaultAsync(p => p.UserId == tutorId);

            var vm = new PanelTutorAsistenciaViewModel
            {
                DiaActualNombre = diaActualNombre,
                HoraActualTexto = now.ToString("HH:mm"),
                HayHorarioEnCurso = false
            };

            if (perfil == null)
            {
                return vm;
            }

            vm.HorariosRegistrados = perfil.Horarios
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
                    MateriaNombre = h.Materia != null ? h.Materia.Nombre : "Cualquier materia asignada",
                    Activo = h.Activo
                })
                .ToList();

            vm.MateriasImpartidas = perfil.MateriasImpartidas
                .Where(m => m.Materia != null)
                .Select(m => new MateriaItemViewModel
                {
                    Id = m.Materia!.Id,
                    Clave = m.Materia.Clave,
                    Nombre = m.Materia.Nombre,
                    Semestre = m.Materia.Semestre,
                    CarreraNombre = string.Empty
                })
                .ToList();

            if (!diaActual.HasValue)
            {
                return vm;
            }

            // Comprobación estricta de horario en curso (Hora del Centro de México)
            var horarioEnCurso = perfil.Horarios
                .FirstOrDefault(h => h.Activo
                                  && h.DiaSemana == diaActual.Value
                                  && h.HoraInicio <= horaActual
                                  && horaActual <= h.HoraFin);

            if (horarioEnCurso == null)
            {
                return vm;
            }

            vm.HayHorarioEnCurso = true;
            vm.HorarioEnCurso = new HorarioEnCursoViewModel
            {
                HorarioId = horarioEnCurso.Id,
                DiaSemana = horarioEnCurso.DiaSemana,
                DiaSemanaNombre = diaActualNombre,
                HoraInicio = horarioEnCurso.HoraInicio,
                HoraFin = horarioEnCurso.HoraFin,
                Lugar = horarioEnCurso.Lugar,
                MateriaId = horarioEnCurso.MateriaId,
                MateriaNombre = horarioEnCurso.Materia != null ? horarioEnCurso.Materia.Nombre : null
            };

            // Buscar si ya existe una sesión de hoy en este bloque (Fecha Centro de México)
            var inicioHoy = TimeHelper.TodayMexico.Add(horarioEnCurso.HoraInicio);
            var finHoy = TimeHelper.TodayMexico.Add(horarioEnCurso.HoraFin);

            var sesionHoy = await _context.SesionesTutoria
                .Include(s => s.Materia)
                .Include(s => s.Asistencias)
                    .ThenInclude(a => a.Alumno)
                        .ThenInclude(al => al!.Carrera)
                .FirstOrDefaultAsync(s => s.TutorId == tutorId
                                       && s.FechaHoraInicio == inicioHoy
                                       && s.FechaHoraFin == finHoy);

            if (sesionHoy != null)
            {
                vm.SesionActual = new SesionTutoriaItemViewModel
                {
                    Id = sesionHoy.Id,
                    Titulo = sesionHoy.Titulo,
                    MateriaId = sesionHoy.MateriaId,
                    MateriaNombre = sesionHoy.Materia != null ? sesionHoy.Materia.Nombre : "Materia no especificada",
                    FechaHoraInicio = sesionHoy.FechaHoraInicio,
                    FechaHoraFin = sesionHoy.FechaHoraFin,
                    Lugar = sesionHoy.Lugar,
                    Activa = sesionHoy.Activa,
                    CapacidadMaxima = sesionHoy.CapacidadMaxima,
                    TotalAsistencias = sesionHoy.Asistencias.Count,
                    TotalConfirmadas = sesionHoy.Asistencias.Count(a => a.Confirmada)
                };

                vm.AsistenciasDeHoy = sesionHoy.Asistencias
                    .OrderByDescending(a => a.FechaHoraRegistro)
                    .Select(a => new AsistenciaItemViewModel
                    {
                        Id = a.Id,
                        SesionId = a.SesionId,
                        TituloSesion = sesionHoy.Titulo,
                        MateriaNombre = sesionHoy.Materia != null ? sesionHoy.Materia.Nombre : string.Empty,
                        TutorNombre = string.Empty,
                        Lugar = sesionHoy.Lugar,
                        AlumnoId = a.AlumnoId,
                        AlumnoNombre = a.Alumno != null ? a.Alumno.NombreCompleto : "Alumno no encontrado",
                        AlumnoBoleta = a.Alumno != null ? a.Alumno.Boleta : string.Empty,
                        AlumnoCarrera = a.Alumno != null && a.Alumno.Carrera != null ? a.Alumno.Carrera.Nombre : (a.Alumno != null && a.Alumno.Academia != null ? a.Alumno.Academia : "UPIICSA"),
                        FechaHoraRegistro = TimeHelper.ConvertToMexico(a.FechaHoraRegistro),
                        Confirmada = a.Confirmada,
                        FechaHoraConfirmacion = a.FechaHoraConfirmacion.HasValue ? TimeHelper.ConvertToMexico(a.FechaHoraConfirmacion.Value) : null,
                        Metodo = a.Metodo,
                        CalificacionEncuesta = a.CalificacionEncuesta,
                        ComentarioEncuesta = a.ComentarioEncuesta
                    })
                    .ToList();
            }

            return vm;
        }

        public async Task<(bool Success, string? ErrorMessage)> RegistrarAsistenciaAutomaticaAsync(string tutorId, string alumnoId, int? materiaId)
        {
            var now = TimeHelper.NowMexico;
            DiaSemana? diaActual = now.DayOfWeek switch
            {
                DayOfWeek.Monday => DiaSemana.Lunes,
                DayOfWeek.Tuesday => DiaSemana.Martes,
                DayOfWeek.Wednesday => DiaSemana.Miercoles,
                DayOfWeek.Thursday => DiaSemana.Jueves,
                DayOfWeek.Friday => DiaSemana.Viernes,
                _ => null
            };

            if (!diaActual.HasValue)
            {
                return (false, "El registro de asistencias únicamente puede realizarse en días hábiles (lunes a viernes).");
            }

            var horaActual = now.TimeOfDay;

            var perfil = await _context.TutoresPerfiles
                .Include(p => p.Horarios.Where(h => h.Activo))
                    .ThenInclude(h => h.Materia)
                .Include(p => p.MateriasImpartidas)
                    .ThenInclude(m => m.Materia)
                .FirstOrDefaultAsync(p => p.UserId == tutorId);

            if (perfil == null)
            {
                return (false, "No se encontró el perfil del tutor en el sistema.");
            }

            // Comprobación estricta de horario en curso (Hora del Centro de México)
            var horario = perfil.Horarios
                .FirstOrDefault(h => h.Activo
                                  && h.DiaSemana == diaActual.Value
                                  && h.HoraInicio <= horaActual
                                  && horaActual <= h.HoraFin);

            if (horario == null)
            {
                return (false, "No tienes ningún horario de tutoría en curso en este momento. El registro solo está habilitado durante tus horarios oficiales de atención.");
            }

            int materiaIdFinal;
            if (horario.MateriaId.HasValue)
            {
                materiaIdFinal = horario.MateriaId.Value;
            }
            else
            {
                if (!materiaId.HasValue || materiaId.Value <= 0)
                {
                    return (false, "Debes seleccionar la materia correspondiente a la sesión de tutoría.");
                }

                bool imparteMateria = perfil.MateriasImpartidas.Any(m => m.MateriaId == materiaId.Value);
                if (!imparteMateria)
                {
                    return (false, "La materia seleccionada no está registrada entre las materias que impartes.");
                }

                materiaIdFinal = materiaId.Value;
            }

            if (tutorId == alumnoId)
            {
                return (false, "Un tutor no puede registrarse asistencia a sí mismo.");
            }

            var alumno = await _context.Users.FirstOrDefaultAsync(u => u.Id == alumnoId && u.Activo);
            if (alumno == null)
            {
                return (false, "El alumno seleccionado no se encuentra activo o no existe.");
            }

            var inicioHoy = TimeHelper.TodayMexico.Add(horario.HoraInicio);
            var finHoy = TimeHelper.TodayMexico.Add(horario.HoraFin);

            var sesion = await _context.SesionesTutoria
                .Include(s => s.Asistencias)
                .FirstOrDefaultAsync(s => s.TutorId == tutorId
                                       && s.MateriaId == materiaIdFinal
                                       && s.FechaHoraInicio == inicioHoy
                                       && s.FechaHoraFin == finHoy);

            if (sesion == null)
            {
                var materiaObj = await _context.Materias.FindAsync(materiaIdFinal);
                var materiaNombre = materiaObj?.Nombre ?? "Materia";

                var tutorUser = await _context.Users.FindAsync(tutorId);
                var tipoTutoria = (tutorUser != null && tutorUser.TipoTutor == TipoTutor.AlumnoTutor)
                    ? TipoTutoria.EntrePares
                    : TipoTutoria.IndividualDocente;

                sesion = new SesionTutoria
                {
                    Titulo = $"Tutoría de {materiaNombre}",
                    MateriaId = materiaIdFinal,
                    TutorId = tutorId,
                    TipoTutoria = tipoTutoria,
                    FechaHoraInicio = inicioHoy,
                    FechaHoraFin = finHoy,
                    Lugar = horario.Lugar,
                    CapacidadMaxima = 10,
                    Activa = true
                };

                _context.SesionesTutoria.Add(sesion);
                await _context.SaveChangesAsync();
            }

            if (!sesion.Activa)
            {
                return (false, "La sesión correspondiente a este horario se encuentra inactiva.");
            }

            var yaRegistrado = await _context.Asistencias
                .AnyAsync(a => a.SesionId == sesion.Id && a.AlumnoId == alumnoId);

            if (yaRegistrado)
            {
                return (false, $"El alumno {alumno.NombreCompleto} ya cuenta con una asistencia registrada en esta sesión.");
            }

            if (sesion.Asistencias.Count >= sesion.CapacidadMaxima)
            {
                return (false, $"La sesión ha alcanzado su capacidad máxima permitida de {sesion.CapacidadMaxima} alumnos.");
            }

            var asistencia = new Asistencia
            {
                SesionId = sesion.Id,
                AlumnoId = alumnoId,
                FechaHoraRegistro = DateTime.UtcNow,
                Metodo = MetodoAsistencia.ManualSignalR,
                Confirmada = false,
                FechaHoraConfirmacion = null
            };

            _context.Asistencias.Add(asistencia);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<MisAsistenciasViewModel> GetMisAsistenciasAsync(string alumnoId)
        {
            var asistencias = await _context.Asistencias
                .Include(a => a.Sesion)
                    .ThenInclude(s => s!.Materia)
                .Include(a => a.Sesion)
                    .ThenInclude(s => s!.Tutor)
                .Where(a => a.AlumnoId == alumnoId)
                .OrderByDescending(a => a.FechaHoraRegistro)
                .ToListAsync();

            var vm = new MisAsistenciasViewModel();

            foreach (var a in asistencias)
            {
                if (a.Sesion == null) continue;

                var materiaNombre = a.Sesion.Materia?.Nombre ?? "Materia no especificada";
                var tutorNombre = a.Sesion.Tutor?.NombreCompleto ?? "Tutor";

                if (!a.Confirmada)
                {
                    vm.Pendientes.Add(new AsistenciaPendienteAlumnoViewModel
                    {
                        AsistenciaId = a.Id,
                        SesionId = a.SesionId,
                        TituloSesion = a.Sesion.Titulo,
                        MateriaNombre = materiaNombre,
                        TutorNombre = tutorNombre,
                        Lugar = a.Sesion.Lugar,
                        FechaHoraRegistro = TimeHelper.ConvertToMexico(a.FechaHoraRegistro),
                        Metodo = a.Metodo
                    });
                }
                else
                {
                    vm.Confirmadas.Add(new AsistenciaItemViewModel
                    {
                        Id = a.Id,
                        SesionId = a.SesionId,
                        TituloSesion = a.Sesion.Titulo,
                        MateriaNombre = materiaNombre,
                        TutorNombre = tutorNombre,
                        Lugar = a.Sesion.Lugar,
                        AlumnoId = a.AlumnoId,
                        AlumnoNombre = string.Empty,
                        AlumnoBoleta = string.Empty,
                        AlumnoCarrera = string.Empty,
                        FechaHoraRegistro = TimeHelper.ConvertToMexico(a.FechaHoraRegistro),
                        Confirmada = a.Confirmada,
                        FechaHoraConfirmacion = a.FechaHoraConfirmacion.HasValue ? TimeHelper.ConvertToMexico(a.FechaHoraConfirmacion.Value) : null,
                        Metodo = a.Metodo,
                        CalificacionEncuesta = a.CalificacionEncuesta,
                        ComentarioEncuesta = a.ComentarioEncuesta
                    });
                }
            }

            return vm;
        }

        public async Task<(bool Success, string? ErrorMessage)> ConfirmarAsistenciaAsync(int asistenciaId, string alumnoId)
        {
            var asistencia = await _context.Asistencias
                .Include(a => a.Sesion)
                .FirstOrDefaultAsync(a => a.Id == asistenciaId && a.AlumnoId == alumnoId);

            if (asistencia == null)
            {
                return (false, "El registro de asistencia no fue encontrado o no corresponde a tu cuenta.");
            }

            if (asistencia.Confirmada)
            {
                return (false, "Esta asistencia ya había sido confirmada previamente.");
            }

            asistencia.Confirmada = true;
            asistencia.FechaHoraConfirmacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> RechazarAsistenciaAsync(int asistenciaId, string alumnoId)
        {
            var asistencia = await _context.Asistencias
                .FirstOrDefaultAsync(a => a.Id == asistenciaId && a.AlumnoId == alumnoId);

            if (asistencia == null)
            {
                return (false, "El registro de asistencia no fue encontrado o no corresponde a tu cuenta.");
            }

            if (asistencia.Confirmada)
            {
                return (false, "No es posible rechazar una asistencia que ya fue confirmada.");
            }

            _context.Asistencias.Remove(asistencia);
            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}
