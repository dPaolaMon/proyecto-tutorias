using System;
using System.Collections.Generic;

namespace tutorias.Models.ViewModels
{
    public class PanelEncuestasViewModel
    {
        // Indicadores Ejecutivos (KPIs)
        public double PromedioGeneral { get; set; }
        public int TotalAsistenciasConfirmadas { get; set; }
        public int TotalAsistenciasEvaluadas { get; set; }

        // Distribución por estrellas
        public int TotalEstrellas5 { get; set; }
        public int TotalEstrellas4 { get; set; }
        public int TotalEstrellas3 { get; set; }
        public int TotalEstrellas2 { get; set; }
        public int TotalEstrellas1 { get; set; }

        public double PorcentajeEstrellas5 => TotalAsistenciasEvaluadas > 0 ? Math.Round((TotalEstrellas5 * 100.0) / TotalAsistenciasEvaluadas, 1) : 0;
        public double PorcentajeEstrellas4 => TotalAsistenciasEvaluadas > 0 ? Math.Round((TotalEstrellas4 * 100.0) / TotalAsistenciasEvaluadas, 1) : 0;
        public double PorcentajeEstrellas3 => TotalAsistenciasEvaluadas > 0 ? Math.Round((TotalEstrellas3 * 100.0) / TotalAsistenciasEvaluadas, 1) : 0;
        public double PorcentajeEstrellas2 => TotalAsistenciasEvaluadas > 0 ? Math.Round((TotalEstrellas2 * 100.0) / TotalAsistenciasEvaluadas, 1) : 0;
        public double PorcentajeEstrellas1 => TotalAsistenciasEvaluadas > 0 ? Math.Round((TotalEstrellas1 * 100.0) / TotalAsistenciasEvaluadas, 1) : 0;

        // Áreas de Oportunidad
        public List<TutorAtencionPrioritariaViewModel> TutoresAtencionPrioritaria { get; set; } = new();
        public List<MateriaMenorSatisfaccionViewModel> MateriasMenorSatisfaccion { get; set; } = new();
        public List<SesionCriticaViewModel> SesionesCriticasRecientes { get; set; } = new();

        // Desempeño por Carrera y Materia
        public List<DesempenoCarreraViewModel> DesempenoPorCarrera { get; set; } = new();
        public List<DesempenoMateriaViewModel> DesempenoPorMateria { get; set; } = new();
    }

    public class TutorAtencionPrioritariaViewModel
    {
        public string TutorId { get; set; } = string.Empty;
        public string TutorNombre { get; set; } = string.Empty;
        public string TipoTutorTexto { get; set; } = string.Empty;
        public double PromedioCalificacion { get; set; }
        public int TotalEvaluaciones { get; set; }
        public int TotalEvaluacionesBajas { get; set; } // Calificaciones de 1 y 2 estrellas
        public string MotivoAlerta { get; set; } = string.Empty;
    }

    public class MateriaMenorSatisfaccionViewModel
    {
        public int MateriaId { get; set; }
        public string Clave { get; set; } = string.Empty;
        public string MateriaNombre { get; set; } = string.Empty;
        public double PromedioCalificacion { get; set; }
        public int TotalEvaluaciones { get; set; }
    }

    public class SesionCriticaViewModel
    {
        public int AsistenciaId { get; set; }
        public DateTime FechaHora { get; set; }
        public string FechaHoraTexto => FechaHora.ToString("dd/MM/yyyy HH:mm");
        public int Calificacion { get; set; }
        public string MateriaNombre { get; set; } = string.Empty;
        public string TutorNombre { get; set; } = string.Empty;
        public string AlumnoNombre { get; set; } = string.Empty;
        public string AlumnoCarrera { get; set; } = string.Empty;
    }

    public class DesempenoCarreraViewModel
    {
        public int? CarreraId { get; set; }
        public string CarreraNombre { get; set; } = string.Empty;
        public int TotalAsistenciasConfirmadas { get; set; }
        public int TotalEvaluaciones { get; set; }
        public double PromedioCalificacion { get; set; }
    }

    public class DesempenoMateriaViewModel
    {
        public int MateriaId { get; set; }
        public string Clave { get; set; } = string.Empty;
        public string MateriaNombre { get; set; } = string.Empty;
        public int TotalEvaluaciones { get; set; }
        public double PromedioCalificacion { get; set; }
    }
}
