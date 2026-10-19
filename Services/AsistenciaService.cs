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
        private readonly IQrCodeService _qrCodeService;

        public AsistenciaService(ApplicationDbContext context, IQrCodeService qrCodeService)
        {
            _context = context;
            _qrCodeService = qrCodeService;
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
            DiaSemana? diaActual = ObtenerDiaSemana(now.DayOfWeek);
            string diaActualNombre = ObtenerNombreDia(now.DayOfWeek);
            var horaActual = now.TimeOfDay;

            var perfil = await _context.TutoresPerfiles
                .Include(p => p.User)
                .Include(p => p.Horarios.Where(h => h.Activo))
                    .ThenInclude(h => h.Materia)
                .Include(p => p.MateriasImpartidas)
                    .ThenInclude(m => m.Materia)
                .FirstOrDefaultAsync(p => p.UserId == tutorId);

            var esDocente = perfil?.User != null && perfil.User.TipoTutor == TipoTutor.DocenteTutor;

            var vm = new PanelTutorAsistenciaViewModel
            {
                DiaActualNombre = diaActualNombre,
                HoraActualTexto = now.ToString("HH:mm"),
                HayHorarioEnCurso = false,
                EsDocente = esDocente
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
            var (success, sesion, error) = await ObtenerOCrearSesionEnCursoAsync(tutorId, materiaId);
            if (!success || sesion == null)
            {
                return (false, error);
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

            var yaRegistrado = await _context.Asistencias
                .AnyAsync(a => a.SesionId == sesion.Id && a.AlumnoId == alumnoId);

            if (yaRegistrado)
            {
                return (false, $"El alumno {alumno.NombreCompleto} ya cuenta con una asistencia registrada en esta sesión.");
            }

            // La restricción de capacidad de 10 alumnos aplica estrictamente y de manera exclusiva a docentes (IndividualDocente).
            // Las tutorías entre pares (alumnos tutores) no tienen ningún límite de cupo.
            if (sesion.TipoTutoria == TipoTutoria.IndividualDocente)
            {
                var totalAsistencias = await _context.Asistencias.CountAsync(a => a.SesionId == sesion.Id);
                if (totalAsistencias >= 10)
                {
                    return (false, "La sesión ha alcanzado su capacidad máxima permitida de 10 alumnos para tutoría docente.");
                }
            }

            var asistencia = new Asistencia
            {
                SesionId = sesion.Id,
                AlumnoId = alumnoId,
                FechaHoraRegistro = TimeHelper.UtcNow,
                Metodo = MetodoAsistencia.ManualSignalR,
                Confirmada = false,
                FechaHoraConfirmacion = null
            };

            _context.Asistencias.Add(asistencia);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, SesionQrDetalleViewModel? Detalle, string? ErrorMessage)> ObtenerOGenerarTokenQrAsync(string tutorId, int? materiaId, string baseUrl)
        {
            var (success, sesion, error) = await ObtenerOCrearSesionEnCursoAsync(tutorId, materiaId);
            if (!success || sesion == null)
            {
                return (false, null, error);
            }

            var ahoraUtc = TimeHelper.UtcNow;

            // Buscar si ya existe un token activo con vigencia menor a 20 min
            var tokenActivo = await _context.SesionesQrTokens
                .Where(t => t.SesionId == sesion.Id && t.Activo && ahoraUtc < t.FechaExpiracion)
                .OrderByDescending(t => t.FechaCreacion)
                .FirstOrDefaultAsync();

            if (tokenActivo == null)
            {
                // Desactivar tokens previos de la sesión
                var anteriores = await _context.SesionesQrTokens
                    .Where(t => t.SesionId == sesion.Id && t.Activo)
                    .ToListAsync();

                foreach (var ant in anteriores)
                {
                    ant.Activo = false;
                }

                // Generar token dinámico nuevo con 20 minutos de vigencia
                tokenActivo = new SesionQrToken
                {
                    SesionId = sesion.Id,
                    Token = Guid.NewGuid().ToString("N"),
                    FechaCreacion = ahoraUtc,
                    FechaExpiracion = ahoraUtc.AddMinutes(20),
                    Activo = true
                };

                _context.SesionesQrTokens.Add(tokenActivo);
                await _context.SaveChangesAsync();
            }

            return await ConstruirDetalleQrAsync(sesion, tokenActivo, baseUrl);
        }

        public async Task<(bool Success, SesionQrDetalleViewModel? Detalle, string? ErrorMessage)> RegenerarTokenQrAsync(string tutorId, int? materiaId, string baseUrl)
        {
            var (success, sesion, error) = await ObtenerOCrearSesionEnCursoAsync(tutorId, materiaId);
            if (!success || sesion == null)
            {
                return (false, null, error);
            }

            var ahoraUtc = TimeHelper.UtcNow;

            // Desactivar todos los tokens activos previos
            var anteriores = await _context.SesionesQrTokens
                .Where(t => t.SesionId == sesion.Id && t.Activo)
                .ToListAsync();

            foreach (var ant in anteriores)
            {
                ant.Activo = false;
            }

            var nuevoToken = new SesionQrToken
            {
                SesionId = sesion.Id,
                Token = Guid.NewGuid().ToString("N"),
                FechaCreacion = ahoraUtc,
                FechaExpiracion = ahoraUtc.AddMinutes(20),
                Activo = true
            };

            _context.SesionesQrTokens.Add(nuevoToken);
            await _context.SaveChangesAsync();

            return await ConstruirDetalleQrAsync(sesion, nuevoToken, baseUrl);
        }

        public async Task<(bool Success, AsistenciaQrResultadoViewModel? Resultado, string? ErrorMessage)> RegistrarAsistenciaPorQrAsync(string alumnoId, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return (false, null, "El token de asistencia QR proporcionado no es válido.");
            }

            var tokenLimpio = token.Trim();
            var qrToken = await _context.SesionesQrTokens
                .Include(t => t.Sesion)
                    .ThenInclude(s => s!.Materia)
                .Include(t => t.Sesion)
                    .ThenInclude(s => s!.Tutor)
                .FirstOrDefaultAsync(t => t.Token == tokenLimpio);

            if (qrToken == null || !qrToken.Activo)
            {
                return (false, null, "El código QR no es válido o ya ha sido desactivado.");
            }

            var ahoraUtc = TimeHelper.UtcNow;

            // Validación estricta de la ventana de 20 minutos
            if (ahoraUtc > qrToken.FechaExpiracion)
            {
                qrToken.Activo = false;
                await _context.SaveChangesAsync();
                return (false, null, "El código QR ha expirado (duración máxima de 20 minutos). Solicita al tutor que proyecte el código QR actualizado.");
            }

            if (qrToken.Sesion == null || !qrToken.Sesion.Activa)
            {
                return (false, null, "La sesión de tutoría asociada a este código QR no se encuentra activa.");
            }

            if (qrToken.Sesion.TutorId == alumnoId)
            {
                return (false, null, "Un tutor no puede registrar asistencia a su propia sesión.");
            }

            var alumno = await _context.Users.FirstOrDefaultAsync(u => u.Id == alumnoId && u.Activo);
            if (alumno == null)
            {
                return (false, null, "El alumno no se encuentra activo en el sistema o no existe.");
            }

            // Validar que el alumno no se haya registrado ya en esta sesión
            var yaRegistrado = await _context.Asistencias
                .AnyAsync(a => a.SesionId == qrToken.SesionId && a.AlumnoId == alumnoId);

            if (yaRegistrado)
            {
                var materiaNom = qrToken.Sesion.Materia?.Nombre ?? "esta tutoría";
                return (false, null, $"Ya tienes una asistencia registrada previamente en esta sesión de {materiaNom}.");
            }

            // La restricción de capacidad de 10 alumnos aplica estrictamente y de manera exclusiva a docentes (IndividualDocente).
            // Las tutorías entre pares (alumnos tutores) no tienen ningún límite de cupo.
            if (qrToken.Sesion.TipoTutoria == TipoTutoria.IndividualDocente)
            {
                var totalAsistencias = await _context.Asistencias.CountAsync(a => a.SesionId == qrToken.SesionId);
                if (totalAsistencias >= 10)
                {
                    return (false, null, "La sesión de tutoría ha alcanzado el límite máximo de 10 alumnos para tutoría docente.");
                }
            }

            // Registro automático directo y confirmado (sin doble validación)
            var asistencia = new Asistencia
            {
                SesionId = qrToken.SesionId,
                AlumnoId = alumnoId,
                FechaHoraRegistro = ahoraUtc,
                Metodo = MetodoAsistencia.CodigoQR,
                Confirmada = true,
                FechaHoraConfirmacion = ahoraUtc
            };

            _context.Asistencias.Add(asistencia);
            await _context.SaveChangesAsync();

            var ahoraMexico = TimeHelper.ConvertToMexico(ahoraUtc);

            return (true, new AsistenciaQrResultadoViewModel
            {
                AsistenciaId = asistencia.Id,
                MateriaNombre = qrToken.Sesion.Materia?.Nombre ?? "Materia de Tutoría",
                TutorNombre = qrToken.Sesion.Tutor?.NombreCompleto ?? "Tutor",
                Lugar = qrToken.Sesion.Lugar,
                FechaHoraRegistroTexto = ahoraMexico.ToString("dd/MM/yyyy HH:mm"),
                AlumnoNombre = alumno.NombreCompleto,
                AlumnoBoleta = alumno.Boleta
            }, null);
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
            asistencia.FechaHoraConfirmacion = TimeHelper.UtcNow;

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

        private async Task<(bool Success, SesionTutoria? Sesion, string? ErrorMessage)> ObtenerOCrearSesionEnCursoAsync(string tutorId, int? materiaId)
        {
            var now = TimeHelper.NowMexico;
            DiaSemana? diaActual = ObtenerDiaSemana(now.DayOfWeek);

            if (!diaActual.HasValue)
            {
                return (false, null, "El registro de asistencias únicamente puede realizarse en días hábiles (lunes a viernes).");
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
                return (false, null, "No se encontró el perfil del tutor en el sistema.");
            }

            var horario = perfil.Horarios
                .FirstOrDefault(h => h.Activo
                                  && h.DiaSemana == diaActual.Value
                                  && h.HoraInicio <= horaActual
                                  && horaActual <= h.HoraFin);

            if (horario == null)
            {
                return (false, null, "No tienes ningún horario de tutoría en curso en este momento. La sesión solo está habilitada durante tus horarios oficiales de atención.");
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
                    if (perfil.MateriasImpartidas.Count == 1)
                    {
                        materiaIdFinal = perfil.MateriasImpartidas.First().MateriaId;
                    }
                    else
                    {
                        return (false, null, "Debes seleccionar la materia correspondiente a la sesión de tutoría.");
                    }
                }
                else
                {
                    bool imparteMateria = perfil.MateriasImpartidas.Any(m => m.MateriaId == materiaId.Value);
                    if (!imparteMateria)
                    {
                        return (false, null, "La materia seleccionada no está registrada entre las materias que impartes.");
                    }

                    materiaIdFinal = materiaId.Value;
                }
            }

            var inicioHoy = TimeHelper.TodayMexico.Add(horario.HoraInicio);
            var finHoy = TimeHelper.TodayMexico.Add(horario.HoraFin);

            var sesion = await _context.SesionesTutoria
                .Include(s => s.Materia)
                .Include(s => s.Tutor)
                .FirstOrDefaultAsync(s => s.TutorId == tutorId
                                       && s.MateriaId == materiaIdFinal
                                       && s.FechaHoraInicio == inicioHoy
                                       && s.FechaHoraFin == finHoy);

            if (sesion == null)
            {
                var materiaObj = await _context.Materias.FindAsync(materiaIdFinal);
                var materiaNombre = materiaObj?.Nombre ?? "Materia";

                var tutorUser = await _context.Users.FindAsync(tutorId);
                var esDocente = (tutorUser == null || tutorUser.TipoTutor != TipoTutor.AlumnoTutor);
                var tipoTutoria = esDocente ? TipoTutoria.IndividualDocente : TipoTutoria.EntrePares;

                sesion = new SesionTutoria
                {
                    Titulo = $"Tutoría de {materiaNombre}",
                    MateriaId = materiaIdFinal,
                    TutorId = tutorId,
                    TipoTutoria = tipoTutoria,
                    FechaHoraInicio = inicioHoy,
                    FechaHoraFin = finHoy,
                    Lugar = horario.Lugar,
                    CapacidadMaxima = esDocente ? 10 : 0,
                    Activa = true
                };

                _context.SesionesTutoria.Add(sesion);
                await _context.SaveChangesAsync();

                // Recargar navegación
                await _context.Entry(sesion).Reference(s => s.Materia).LoadAsync();
                await _context.Entry(sesion).Reference(s => s.Tutor).LoadAsync();
            }

            if (!sesion.Activa)
            {
                return (false, null, "La sesión correspondiente a este horario se encuentra inactiva.");
            }

            return (true, sesion, null);
        }

        private async Task<(bool Success, SesionQrDetalleViewModel? Detalle, string? ErrorMessage)> ConstruirDetalleQrAsync(SesionTutoria sesion, SesionQrToken token, string baseUrl)
        {
            var urlValidacion = $"{baseUrl.TrimEnd('/')}/Asistencia/ValidarQr?token={token.Token}";
            var qrBase64 = _qrCodeService.GenerarQrPngBase64(urlValidacion);
            var segundosRestantes = Math.Max(0, (int)(token.FechaExpiracion - TimeHelper.UtcNow).TotalSeconds);

            var asistencias = await _context.Asistencias
                .Include(a => a.Alumno)
                    .ThenInclude(al => al!.Carrera)
                .Where(a => a.SesionId == sesion.Id)
                .OrderByDescending(a => a.FechaHoraRegistro)
                .Select(a => new AsistenciaItemViewModel
                {
                    Id = a.Id,
                    SesionId = a.SesionId,
                    TituloSesion = sesion.Titulo,
                    MateriaNombre = sesion.Materia != null ? sesion.Materia.Nombre : string.Empty,
                    Lugar = sesion.Lugar,
                    AlumnoId = a.AlumnoId,
                    AlumnoNombre = a.Alumno != null ? a.Alumno.NombreCompleto : "Alumno",
                    AlumnoBoleta = a.Alumno != null ? a.Alumno.Boleta : string.Empty,
                    AlumnoCarrera = a.Alumno != null && a.Alumno.Carrera != null ? a.Alumno.Carrera.Nombre : (a.Alumno != null && a.Alumno.Academia != null ? a.Alumno.Academia : "UPIICSA"),
                    FechaHoraRegistro = TimeHelper.ConvertToMexico(a.FechaHoraRegistro),
                    Confirmada = a.Confirmada,
                    FechaHoraConfirmacion = a.FechaHoraConfirmacion.HasValue ? TimeHelper.ConvertToMexico(a.FechaHoraConfirmacion.Value) : null,
                    Metodo = a.Metodo
                })
                .ToListAsync();

            var vm = new SesionQrDetalleViewModel
            {
                SesionId = sesion.Id,
                Titulo = sesion.Titulo,
                MateriaNombre = sesion.Materia?.Nombre ?? "Tutoría",
                TutorNombre = sesion.Tutor?.NombreCompleto ?? "Tutor",
                Lugar = sesion.Lugar,
                Token = token.Token,
                QrBase64 = qrBase64,
                UrlValidacion = urlValidacion,
                SegundosRestantes = segundosRestantes,
                FechaExpiracion = TimeHelper.ConvertToMexico(token.FechaExpiracion),
                Asistencias = asistencias
            };

            return (true, vm, null);
        }

        private static DiaSemana? ObtenerDiaSemana(DayOfWeek day) => day switch
        {
            DayOfWeek.Monday => DiaSemana.Lunes,
            DayOfWeek.Tuesday => DiaSemana.Martes,
            DayOfWeek.Wednesday => DiaSemana.Miercoles,
            DayOfWeek.Thursday => DiaSemana.Jueves,
            DayOfWeek.Friday => DiaSemana.Viernes,
            _ => null
        };

        private static string ObtenerNombreDia(DayOfWeek day) => day switch
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
    }
}
