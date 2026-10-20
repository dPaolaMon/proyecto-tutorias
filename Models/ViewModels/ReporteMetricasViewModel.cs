using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using tutorias.Models;

namespace tutorias.Models.ViewModels
{
    public class ReporteMetricasFiltroViewModel
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? CarreraId { get; set; }
        public TipoTutor? TipoTutor { get; set; }
        public int? MateriaId { get; set; }
        public string? TutorId { get; set; }
    }

    public class MateriaDemandaItemViewModel
    {
        public int MateriaId { get; set; }
        public string MateriaClave { get; set; } = string.Empty;
        public string MateriaNombre { get; set; } = string.Empty;
        public string CarreraNombre { get; set; } = string.Empty;
        public int TotalAsistencias { get; set; }
        public int TotalAlumnosDistintos { get; set; }
        public double PorcentajeTotal { get; set; }
    }

    public class CarreraDistribucionItemViewModel
    {
        public int CarreraId { get; set; }
        public string CarreraNombre { get; set; } = string.Empty;
        public int TotalAsistencias { get; set; }
        public double Porcentaje { get; set; }
    }

    public class ReporteMetricasViewModel
    {
        public ReporteMetricasFiltroViewModel Filtro { get; set; } = new();
        public bool EsCoordinador { get; set; }
        public string PeriodoTexto { get; set; } = string.Empty;
        public DateTime FechaGeneracion { get; set; }

        // Indicadores Globales (RF-29)
        public int TotalAsistenciasConfirmadas { get; set; }
        public int TotalAlumnosBeneficiados { get; set; }
        public int TotalTutoresActivos { get; set; }
        public double PromedioSatisfaccion { get; set; }
        public int TotalEvaluaciones { get; set; }

        // Desglose por Tipo de Tutor (RF-29)
        public int AsistenciasDocenteTutor { get; set; }
        public double PorcentajeDocenteTutor { get; set; }
        public int AsistenciasAlumnoTutor { get; set; }
        public double PorcentajeAlumnoTutor { get; set; }

        // Desglose por Metodo de Asistencia
        public int AsistenciasQr { get; set; }
        public double PorcentajeQr { get; set; }
        public int AsistenciasManual { get; set; }
        public double PorcentajeManual { get; set; }

        // Ranking de Materias con mayor demanda (RF-29)
        public List<MateriaDemandaItemViewModel> RankingMaterias { get; set; } = new();

        // Distribución por Programa Académico
        public List<CarreraDistribucionItemViewModel> DistribucionCarreras { get; set; } = new();

        // Listas para los filtros
        public List<SelectListItem> CarrerasDisponibles { get; set; } = new();
        public List<SelectListItem> MateriasDisponibles { get; set; } = new();
        public List<SelectListItem> TutoresDisponibles { get; set; } = new();
        public List<SelectListItem> TiposTutorDisponibles { get; set; } = new();
    }
}
