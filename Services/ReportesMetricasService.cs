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
    public class ReportesMetricasService : IReportesMetricasService
    {
        private readonly ApplicationDbContext _context;

        public ReportesMetricasService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReporteMetricasViewModel> ObtenerReporteMetricasAsync(
            string usuarioId,
            bool esCoordinador,
            ReporteMetricasFiltroViewModel filtro)
        {
            var ahoraMexico = TimeHelper.NowMexico;

            // Rango de fechas por defecto: primer día del mes actual hasta hoy
            var fechaInicio = filtro.FechaInicio?.Date ?? new DateTime(ahoraMexico.Year, ahoraMexico.Month, 1);
            var fechaFin = filtro.FechaFin?.Date ?? ahoraMexico.Date;

            if (fechaFin < fechaInicio)
            {
                fechaFin = fechaInicio;
            }

            // Convertir a UTC con base en la zona horaria de México
            var tzMexico = TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City");
            var inicioUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(fechaInicio, DateTimeKind.Unspecified), tzMexico);
            var finUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(fechaFin.AddDays(1), DateTimeKind.Unspecified), tzMexico);

            var query = _context.Asistencias
                .Include(a => a.Alumno)
                    .ThenInclude(u => u!.Carrera)
                .Include(a => a.Sesion)
                    .ThenInclude(s => s!.Tutor)
                .Include(a => a.Sesion)
                    .ThenInclude(s => s!.Materia)
                        .ThenInclude(m => m!.Carrera)
                .Where(a => a.Confirmada && a.FechaHoraRegistro >= inicioUtc && a.FechaHoraRegistro < finUtc);

            if (!esCoordinador)
            {
                query = query.Where(a => a.Sesion != null && a.Sesion.TutorId == usuarioId);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(filtro.TutorId))
                {
                    query = query.Where(a => a.Sesion != null && a.Sesion.TutorId == filtro.TutorId);
                }

                if (filtro.CarreraId.HasValue && filtro.CarreraId.Value > 0)
                {
                    query = query.Where(a => (a.Alumno != null && a.Alumno.CarreraId == filtro.CarreraId.Value) ||
                                             (a.Sesion != null && a.Sesion.Materia != null && a.Sesion.Materia.CarreraId == filtro.CarreraId.Value));
                }

                if (filtro.TipoTutor.HasValue && filtro.TipoTutor.Value != TipoTutor.Ninguno)
                {
                    query = query.Where(a => a.Sesion != null && a.Sesion.Tutor != null && a.Sesion.Tutor.TipoTutor == filtro.TipoTutor.Value);
                }

                if (filtro.MateriaId.HasValue && filtro.MateriaId.Value > 0)
                {
                    query = query.Where(a => a.Sesion != null && a.Sesion.MateriaId == filtro.MateriaId.Value);
                }
            }

            var asistencias = await query.ToListAsync();

            var totalAsistencias = asistencias.Count;
            var alumnosBeneficiados = asistencias.Select(a => a.AlumnoId).Distinct().Count();
            var tutoresActivos = asistencias.Where(a => a.Sesion?.TutorId != null).Select(a => a.Sesion!.TutorId).Distinct().Count();

            // Evaluaciones de satisfacción
            var evaluaciones = asistencias.Where(a => a.CalificacionEncuesta.HasValue && a.CalificacionEncuesta.Value > 0).ToList();
            var totalEvaluaciones = evaluaciones.Count;
            var promedioSatisfaccion = totalEvaluaciones > 0
                ? Math.Round(evaluaciones.Average(a => a.CalificacionEncuesta!.Value), 2)
                : 0.0;

            // Desglose por Tipo de Tutor
            var asistenciasDocente = asistencias.Count(a => a.Sesion?.Tutor?.TipoTutor == TipoTutor.DocenteTutor);
            var asistenciasAlumno = asistencias.Count(a => a.Sesion?.Tutor?.TipoTutor == TipoTutor.AlumnoTutor);
            var pctDocente = totalAsistencias > 0 ? Math.Round((double)asistenciasDocente / totalAsistencias * 100.0, 1) : 0.0;
            var pctAlumno = totalAsistencias > 0 ? Math.Round((double)asistenciasAlumno / totalAsistencias * 100.0, 1) : 0.0;

            // Desglose por Método
            var asistenciasQr = asistencias.Count(a => a.Metodo == MetodoAsistencia.CodigoQR);
            var asistenciasManual = asistencias.Count(a => a.Metodo == MetodoAsistencia.ManualSignalR);
            var pctQr = totalAsistencias > 0 ? Math.Round((double)asistenciasQr / totalAsistencias * 100.0, 1) : 0.0;
            var pctManual = totalAsistencias > 0 ? Math.Round((double)asistenciasManual / totalAsistencias * 100.0, 1) : 0.0;

            // Ranking de Materias más demandadas (Top 10)
            var rankingMaterias = asistencias
                .Where(a => a.Sesion?.Materia != null)
                .GroupBy(a => a.Sesion!.MateriaId)
                .Select(g =>
                {
                    var primer = g.First();
                    var materia = primer.Sesion!.Materia!;
                    var carreraNombre = materia.Carrera?.Nombre ?? "UPIICSA";
                    var conteo = g.Count();

                    return new MateriaDemandaItemViewModel
                    {
                        MateriaId = g.Key,
                        MateriaClave = materia.Clave,
                        MateriaNombre = materia.Nombre,
                        CarreraNombre = carreraNombre,
                        TotalAsistencias = conteo,
                        TotalAlumnosDistintos = g.Select(x => x.AlumnoId).Distinct().Count(),
                        PorcentajeTotal = totalAsistencias > 0 ? Math.Round((double)conteo / totalAsistencias * 100.0, 1) : 0.0
                    };
                })
                .OrderByDescending(m => m.TotalAsistencias)
                .Take(10)
                .ToList();

            // Distribución por Carrera
            var distribucionCarreras = asistencias
                .GroupBy(a => a.Alumno?.Carrera != null ? a.Alumno.Carrera.Nombre : (!string.IsNullOrWhiteSpace(a.Alumno?.Academia) ? a.Alumno.Academia : "Sin Asignar"))
                .Select(g =>
                {
                    var conteo = g.Count();
                    return new CarreraDistribucionItemViewModel
                    {
                        CarreraNombre = g.Key,
                        TotalAsistencias = conteo,
                        Porcentaje = totalAsistencias > 0 ? Math.Round((double)conteo / totalAsistencias * 100.0, 1) : 0.0
                    };
                })
                .OrderByDescending(c => c.TotalAsistencias)
                .ToList();

            var periodoTexto = $"{fechaInicio:dd/MM/yyyy} al {fechaFin:dd/MM/yyyy}";

            var vm = new ReporteMetricasViewModel
            {
                Filtro = new ReporteMetricasFiltroViewModel
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    CarreraId = filtro.CarreraId,
                    TipoTutor = filtro.TipoTutor,
                    MateriaId = filtro.MateriaId,
                    TutorId = filtro.TutorId
                },
                EsCoordinador = esCoordinador,
                PeriodoTexto = periodoTexto,
                FechaGeneracion = ahoraMexico,
                TotalAsistenciasConfirmadas = totalAsistencias,
                TotalAlumnosBeneficiados = alumnosBeneficiados,
                TotalTutoresActivos = tutoresActivos,
                PromedioSatisfaccion = promedioSatisfaccion,
                TotalEvaluaciones = totalEvaluaciones,
                AsistenciasDocenteTutor = asistenciasDocente,
                PorcentajeDocenteTutor = pctDocente,
                AsistenciasAlumnoTutor = asistenciasAlumno,
                PorcentajeAlumnoTutor = pctAlumno,
                AsistenciasQr = asistenciasQr,
                PorcentajeQr = pctQr,
                AsistenciasManual = asistenciasManual,
                PorcentajeManual = pctManual,
                RankingMaterias = rankingMaterias,
                DistribucionCarreras = distribucionCarreras
            };

            // Llenar catálogos para filtros
            vm.CarrerasDisponibles = await ObtenerListaCarrerasAsync();
            vm.MateriasDisponibles = await ObtenerListaMateriasAsync();
            vm.TiposTutorDisponibles = ObtenerListaTiposTutor();

            if (esCoordinador)
            {
                vm.TutoresDisponibles = await ObtenerListaTutoresAsync();
            }

            return vm;
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

        private async Task<List<SelectListItem>> ObtenerListaMateriasAsync()
        {
            var materias = await _context.Materias
                .OrderBy(m => m.Nombre)
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = $"{m.Clave} - {m.Nombre}"
                })
                .Take(150)
                .ToListAsync();

            materias.Insert(0, new SelectListItem { Value = "", Text = "-- Todas las materias --" });
            return materias;
        }

        private List<SelectListItem> ObtenerListaTiposTutor()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Todos los tipos de tutor --" },
                new SelectListItem { Value = ((int)TipoTutor.DocenteTutor).ToString(), Text = "Docente Tutor (Tutoría Individual)" },
                new SelectListItem { Value = ((int)TipoTutor.AlumnoTutor).ToString(), Text = "Alumno Tutor (Tutoría entre Pares)" }
            };
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
    }
}
