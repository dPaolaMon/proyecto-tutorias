using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace tutorias.Models.ViewModels
{
    public class ReporteDiarioViewModel
    {
        public DateTime Fecha { get; set; }
        public string FechaTexto { get; set; } = string.Empty;
        public string FechaInput => Fecha.ToString("yyyy-MM-dd");
        public List<ReporteDiarioItemViewModel> Items { get; set; } = new();
        public int TotalAsistencias => Items.Count;
        public bool EsCoordinador { get; set; }
        public string? TutorFiltroId { get; set; }
        public int? CarreraFiltroId { get; set; }
        public List<SelectListItem> TutoresDisponibles { get; set; } = new();
        public List<SelectListItem> CarrerasDisponibles { get; set; } = new();
    }

    public class ReporteDiarioItemViewModel
    {
        public int AsistenciaId { get; set; }
        public DateTime FechaHora { get; set; }
        public string FechaHoraTexto => FechaHora.ToString("dd/MM/yyyy HH:mm");
        public string HoraTexto => FechaHora.ToString("HH:mm");
        public string AlumnoNombreCompleto { get; set; } = string.Empty;
        public string AlumnoBoleta { get; set; } = string.Empty;
        public string ProgramaAcademico { get; set; } = string.Empty;
        public string TutorNombre { get; set; } = string.Empty;
        public string MateriaNombre { get; set; } = string.Empty;
        public string MetodoTexto { get; set; } = string.Empty;
    }

    public class ReporteMensualViewModel
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string MesNombre { get; set; } = string.Empty;
        public string MesInput => $"{Anio:D4}-{Mes:D2}";
        public List<ReporteMensualItemViewModel> Items { get; set; } = new();
        public int TotalAlumnosUnicos => Items.Count;
        public int TotalAsistencias => Items.Sum(i => i.TotalAsistencias);
        public bool EsCoordinador { get; set; }
        public string? TutorFiltroId { get; set; }
        public int? CarreraFiltroId { get; set; }
        public List<SelectListItem> TutoresDisponibles { get; set; } = new();
        public List<SelectListItem> CarrerasDisponibles { get; set; } = new();
    }

    public class ReporteMensualItemViewModel
    {
        public string AlumnoId { get; set; } = string.Empty;
        public string AlumnoNombreCompleto { get; set; } = string.Empty;
        public string AlumnoBoleta { get; set; } = string.Empty;
        public string ProgramaAcademico { get; set; } = string.Empty;
        public int TotalAsistencias { get; set; }
    }
}
