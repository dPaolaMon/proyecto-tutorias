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
    public class EncuestasService : IEncuestasService
    {
        private readonly ApplicationDbContext _context;

        public EncuestasService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PanelEncuestasViewModel> ObtenerPanelEncuestasAsync()
        {
            var totalConfirmadas = await _context.Asistencias
                .CountAsync(a => a.Confirmada);

            var evaluadas = await _context.Asistencias
                .Include(a => a.Alumno)
                    .ThenInclude(al => al!.Carrera)
                .Include(a => a.Sesion)
                    .ThenInclude(s => s!.Materia)
                .Include(a => a.Sesion)
                    .ThenInclude(s => s!.Tutor)
                .Where(a => a.Confirmada && a.CalificacionEncuesta.HasValue)
                .ToListAsync();

            var vm = new PanelEncuestasViewModel
            {
                TotalAsistenciasConfirmadas = totalConfirmadas,
                TotalAsistenciasEvaluadas = evaluadas.Count,
                PromedioGeneral = evaluadas.Any()
                    ? Math.Round(evaluadas.Average(a => a.CalificacionEncuesta!.Value), 2)
                    : 0.0,

                TotalEstrellas5 = evaluadas.Count(a => a.CalificacionEncuesta == 5),
                TotalEstrellas4 = evaluadas.Count(a => a.CalificacionEncuesta == 4),
                TotalEstrellas3 = evaluadas.Count(a => a.CalificacionEncuesta == 3),
                TotalEstrellas2 = evaluadas.Count(a => a.CalificacionEncuesta == 2),
                TotalEstrellas1 = evaluadas.Count(a => a.CalificacionEncuesta == 1)
            };

            // 1. Detección de Áreas de Oportunidad: Tutores con Atención Prioritaria
            var tutoresEvaluados = evaluadas
                .Where(a => a.Sesion != null && !string.IsNullOrWhiteSpace(a.Sesion.TutorId))
                .GroupBy(a => a.Sesion!.TutorId!)
                .Select(g =>
                {
                    var tutor = g.First().Sesion!.Tutor;
                    var promedio = Math.Round(g.Average(a => a.CalificacionEncuesta!.Value), 2);
                    var totalBajas = g.Count(a => a.CalificacionEncuesta <= 2);
                    var tipoTexto = tutor != null && tutor.TipoTutor == TipoTutor.DocenteTutor
                        ? "Docente"
                        : "Alumno Tutor";

                    string motivo;
                    if (promedio < 3.5 && totalBajas >= 2)
                    {
                        motivo = "Promedio menor a 3.5 estrellas y múltiples evaluaciones bajas.";
                    }
                    else if (promedio < 3.5)
                    {
                        motivo = "Promedio inferior al umbral institucional (menor a 3.5 estrellas).";
                    }
                    else
                    {
                        motivo = $"{totalBajas} evaluación(es) con 1 o 2 estrellas.";
                    }

                    return new TutorAtencionPrioritariaViewModel
                    {
                        TutorId = g.Key,
                        TutorNombre = tutor?.NombreCompleto ?? "Tutor",
                        TipoTutorTexto = tipoTexto,
                        PromedioCalificacion = promedio,
                        TotalEvaluaciones = g.Count(),
                        TotalEvaluacionesBajas = totalBajas,
                        MotivoAlerta = motivo
                    };
                })
                .Where(t => t.PromedioCalificacion < 3.5 || t.TotalEvaluacionesBajas >= 2)
                .OrderBy(t => t.PromedioCalificacion)
                .ToList();

            vm.TutoresAtencionPrioritaria = tutoresEvaluados;

            // 2. Detección de Áreas de Oportunidad: Materias con Menor Nivel de Satisfacción
            var materiasEvaluadas = evaluadas
                .Where(a => a.Sesion != null && a.Sesion.Materia != null)
                .GroupBy(a => a.Sesion!.MateriaId)
                .Select(g =>
                {
                    var materia = g.First().Sesion!.Materia!;
                    var promedio = Math.Round(g.Average(a => a.CalificacionEncuesta!.Value), 2);
                    return new MateriaMenorSatisfaccionViewModel
                    {
                        MateriaId = g.Key,
                        Clave = materia.Clave,
                        MateriaNombre = materia.Nombre,
                        PromedioCalificacion = promedio,
                        TotalEvaluaciones = g.Count()
                    };
                })
                .Where(m => m.PromedioCalificacion < 4.0)
                .OrderBy(m => m.PromedioCalificacion)
                .Take(10)
                .ToList();

            vm.MateriasMenorSatisfaccion = materiasEvaluadas;

            // 3. Detección de Áreas de Oportunidad: Sesiones Críticas Recientes (1 o 2 estrellas)
            var criticas = evaluadas
                .Where(a => a.CalificacionEncuesta <= 2)
                .OrderByDescending(a => a.FechaHoraRegistro)
                .Take(10)
                .Select(a =>
                {
                    var carreraNombre = a.Alumno?.Carrera != null
                        ? a.Alumno.Carrera.Nombre
                        : (!string.IsNullOrWhiteSpace(a.Alumno?.Academia) ? a.Alumno.Academia : "UPIICSA");

                    return new SesionCriticaViewModel
                    {
                        AsistenciaId = a.Id,
                        FechaHora = TimeHelper.ConvertToMexico(a.FechaHoraRegistro),
                        Calificacion = a.CalificacionEncuesta!.Value,
                        MateriaNombre = a.Sesion?.Materia?.Nombre ?? "Materia no especificada",
                        TutorNombre = a.Sesion?.Tutor?.NombreCompleto ?? "Tutor",
                        AlumnoNombre = a.Alumno != null ? a.Alumno.NombreCompleto : "Alumno",
                        AlumnoCarrera = carreraNombre
                    };
                })
                .ToList();

            vm.SesionesCriticasRecientes = criticas;

            // 4. Desempeño por Carrera / Programa Académico
            var todasConfirmadas = await _context.Asistencias
                .Include(a => a.Alumno)
                    .ThenInclude(al => al!.Carrera)
                .Where(a => a.Confirmada)
                .ToListAsync();

            var desempenoCarreras = todasConfirmadas
                .GroupBy(a => new
                {
                    CarreraId = a.Alumno != null ? a.Alumno.CarreraId : null,
                    CarreraNombre = a.Alumno?.Carrera != null
                        ? a.Alumno.Carrera.Nombre
                        : (!string.IsNullOrWhiteSpace(a.Alumno?.Academia) ? a.Alumno.Academia : "UPIICSA")
                })
                .Select(g =>
                {
                    var evaluadasCarrera = g.Where(a => a.CalificacionEncuesta.HasValue).ToList();
                    var promedio = evaluadasCarrera.Any()
                        ? Math.Round(evaluadasCarrera.Average(a => a.CalificacionEncuesta!.Value), 2)
                        : 0.0;

                    return new DesempenoCarreraViewModel
                    {
                        CarreraId = g.Key.CarreraId,
                        CarreraNombre = g.Key.CarreraNombre,
                        TotalAsistenciasConfirmadas = g.Count(),
                        TotalEvaluaciones = evaluadasCarrera.Count,
                        PromedioCalificacion = promedio
                    };
                })
                .OrderByDescending(c => c.PromedioCalificacion)
                .ThenByDescending(c => c.TotalEvaluaciones)
                .ToList();

            vm.DesempenoPorCarrera = desempenoCarreras;

            // 5. Desempeño por Materia / Unidad de Aprendizaje
            var desempenoMaterias = evaluadas
                .Where(a => a.Sesion != null && a.Sesion.Materia != null)
                .GroupBy(a => a.Sesion!.MateriaId)
                .Select(g =>
                {
                    var materia = g.First().Sesion!.Materia!;
                    var promedio = Math.Round(g.Average(a => a.CalificacionEncuesta!.Value), 2);

                    return new DesempenoMateriaViewModel
                    {
                        MateriaId = g.Key,
                        Clave = materia.Clave,
                        MateriaNombre = materia.Nombre,
                        TotalEvaluaciones = g.Count(),
                        PromedioCalificacion = promedio
                    };
                })
                .OrderByDescending(m => m.PromedioCalificacion)
                .ThenByDescending(m => m.TotalEvaluaciones)
                .ToList();

            vm.DesempenoPorMateria = desempenoMaterias;

            return vm;
        }
    }
}
