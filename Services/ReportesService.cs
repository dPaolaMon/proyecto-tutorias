using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using tutorias.Data;
using tutorias.Helpers;
using tutorias.Models;
using tutorias.Models.ViewModels;

namespace tutorias.Services
{
    public class ReportesService : IReportesService
    {
        private readonly ApplicationDbContext _context;

        public ReportesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReporteDiarioViewModel> ObtenerReporteDiarioAsync(
            string usuarioId,
            bool esCoordinador,
            DateTime? fecha,
            string? tutorFiltroId,
            int? carreraFiltroId)
        {
            var fechaReal = fecha?.Date ?? TimeHelper.TodayMexico;
            var (inicioUtc, finUtc) = TimeHelper.GetRangoUtcDia(fechaReal);

            var query = _context.Asistencias
                .Include(a => a.Alumno)
                    .ThenInclude(u => u!.Carrera)
                .Include(a => a.Sesion)
                    .ThenInclude(s => s!.Tutor)
                .Include(a => a.Sesion)
                    .ThenInclude(s => s!.Materia)
                .Where(a => a.Confirmada && a.FechaHoraRegistro >= inicioUtc && a.FechaHoraRegistro < finUtc);

            if (!esCoordinador)
            {
                query = query.Where(a => a.Sesion != null && a.Sesion.TutorId == usuarioId);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(tutorFiltroId))
                {
                    query = query.Where(a => a.Sesion != null && a.Sesion.TutorId == tutorFiltroId);
                }

                if (carreraFiltroId.HasValue && carreraFiltroId.Value > 0)
                {
                    query = query.Where(a => a.Alumno != null && a.Alumno.CarreraId == carreraFiltroId.Value);
                }
            }

            var asistencias = await query
                .OrderByDescending(a => a.FechaHoraRegistro)
                .ToListAsync();

            var items = asistencias.Select(a =>
            {
                var carreraNombre = a.Alumno?.Carrera != null
                    ? a.Alumno.Carrera.Nombre
                    : (!string.IsNullOrWhiteSpace(a.Alumno?.Academia) ? a.Alumno.Academia : "UPIICSA");

                var metodoTexto = a.Metodo == MetodoAsistencia.CodigoQR
                    ? "Codigo QR"
                    : "Manual / Solicitud";

                return new ReporteDiarioItemViewModel
                {
                    AsistenciaId = a.Id,
                    FechaHora = TimeHelper.ConvertToMexico(a.FechaHoraRegistro),
                    AlumnoNombreCompleto = a.Alumno != null ? a.Alumno.NombreCompleto : "Alumno no localizado",
                    AlumnoBoleta = a.Alumno?.Boleta ?? string.Empty,
                    ProgramaAcademico = carreraNombre,
                    TutorNombre = a.Sesion?.Tutor?.NombreCompleto ?? "Tutor",
                    MateriaNombre = a.Sesion?.Materia?.Nombre ?? "Materia no especificada",
                    MetodoTexto = metodoTexto
                };
            }).ToList();

            var vm = new ReporteDiarioViewModel
            {
                Fecha = fechaReal,
                FechaTexto = ObtenerFechaTexto(fechaReal),
                Items = items,
                EsCoordinador = esCoordinador,
                TutorFiltroId = tutorFiltroId,
                CarreraFiltroId = carreraFiltroId
            };

            if (esCoordinador)
            {
                vm.TutoresDisponibles = await ObtenerListaTutoresAsync();
                vm.CarrerasDisponibles = await ObtenerListaCarrerasAsync();
            }

            return vm;
        }

        public async Task<ReporteMensualViewModel> ObtenerReporteMensualAsync(
            string usuarioId,
            bool esCoordinador,
            int? anio,
            int? mes,
            string? tutorFiltroId,
            int? carreraFiltroId)
        {
            var ahoraMexico = TimeHelper.NowMexico;
            var anioReal = anio.HasValue && anio.Value >= 2000 && anio.Value <= 2100 ? anio.Value : ahoraMexico.Year;
            var mesReal = mes.HasValue && mes.Value >= 1 && mes.Value <= 12 ? mes.Value : ahoraMexico.Month;

            var (inicioUtc, finUtc) = TimeHelper.GetRangoUtcMes(anioReal, mesReal);

            var query = _context.Asistencias
                .Include(a => a.Alumno)
                    .ThenInclude(u => u!.Carrera)
                .Include(a => a.Sesion)
                .Where(a => a.Confirmada && a.FechaHoraRegistro >= inicioUtc && a.FechaHoraRegistro < finUtc);

            if (!esCoordinador)
            {
                query = query.Where(a => a.Sesion != null && a.Sesion.TutorId == usuarioId);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(tutorFiltroId))
                {
                    query = query.Where(a => a.Sesion != null && a.Sesion.TutorId == tutorFiltroId);
                }

                if (carreraFiltroId.HasValue && carreraFiltroId.Value > 0)
                {
                    query = query.Where(a => a.Alumno != null && a.Alumno.CarreraId == carreraFiltroId.Value);
                }
            }

            var asistencias = await query.ToListAsync();

            var agrupados = asistencias
                .GroupBy(a => a.AlumnoId)
                .Select(g =>
                {
                    var primerRegistro = g.First();
                    var alumno = primerRegistro.Alumno;
                    var carreraNombre = alumno?.Carrera != null
                        ? alumno.Carrera.Nombre
                        : (!string.IsNullOrWhiteSpace(alumno?.Academia) ? alumno.Academia : "UPIICSA");

                    return new ReporteMensualItemViewModel
                    {
                        AlumnoId = g.Key,
                        AlumnoNombreCompleto = alumno != null ? alumno.NombreCompleto : "Alumno no localizado",
                        AlumnoBoleta = alumno?.Boleta ?? string.Empty,
                        ProgramaAcademico = carreraNombre,
                        TotalAsistencias = g.Count()
                    };
                })
                .OrderBy(item => item.AlumnoNombreCompleto)
                .ToList();

            var vm = new ReporteMensualViewModel
            {
                Anio = anioReal,
                Mes = mesReal,
                MesNombre = $"{ObtenerNombreMes(mesReal)} {anioReal}",
                Items = agrupados,
                EsCoordinador = esCoordinador,
                TutorFiltroId = tutorFiltroId,
                CarreraFiltroId = carreraFiltroId
            };

            if (esCoordinador)
            {
                vm.TutoresDisponibles = await ObtenerListaTutoresAsync();
                vm.CarrerasDisponibles = await ObtenerListaCarrerasAsync();
            }

            return vm;
        }

        private async Task<List<SelectListItem>> ObtenerListaTutoresAsync()
        {
            var tutores = await _context.Users
                .Where(u => u.Activo && (u.TipoTutor == TipoTutor.DocenteTutor || u.TipoTutor == TipoTutor.AlumnoTutor))
                .OrderBy(u => u.Nombre)
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = $"{u.Nombre} {u.ApellidoPaterno} {u.ApellidoMaterno}".Trim() + (u.TipoTutor == TipoTutor.DocenteTutor ? " (Docente)" : " (Alumno Tutor)")
                })
                .ToListAsync();

            tutores.Insert(0, new SelectListItem { Value = "", Text = "-- Todos los tutores --" });
            return tutores;
        }

        private async Task<List<SelectListItem>> ObtenerListaCarrerasAsync()
        {
            var carreras = await _context.Carreras
                .OrderBy(c => c.Nombre)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Nombre
                })
                .ToListAsync();

            carreras.Insert(0, new SelectListItem { Value = "", Text = "-- Todas las carreras --" });
            return carreras;
        }

        private static string ObtenerFechaTexto(DateTime fecha)
        {
            var culture = new CultureInfo("es-MX");
            var nombreDia = culture.DateTimeFormat.GetDayName(fecha.DayOfWeek);
            var nombreMes = culture.DateTimeFormat.GetMonthName(fecha.Month);
            nombreDia = char.ToUpper(nombreDia[0]) + nombreDia.Substring(1);
            return $"{nombreDia}, {fecha.Day} de {nombreMes} de {fecha.Year}";
        }

        private static string ObtenerNombreMes(int mes)
        {
            var culture = new CultureInfo("es-MX");
            var nombre = culture.DateTimeFormat.GetMonthName(mes);
            return char.ToUpper(nombre[0]) + nombre.Substring(1);
        }
    }
}
