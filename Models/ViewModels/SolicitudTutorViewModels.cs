using System;
using tutorias.Models;

namespace tutorias.Models.ViewModels
{
    public class SolicitudTutorViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Boleta { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool EsDocente { get; set; }
        public string ProgramaOAcademia { get; set; } = string.Empty;
        public bool PerfilCompleto { get; set; }

        public TipoTutor TipoTutorSolicitado { get; set; }

        public string ModalidadTitulo => TipoTutorSolicitado == TipoTutor.AlumnoTutor
            ? "Tutoría entre Pares"
            : "Tutoría Individual";

        public string ModalidadDescripcion => TipoTutorSolicitado == TipoTutor.AlumnoTutor
            ? "Modalidad para alumnos que acompañan y orientan académicamente a sus compañeros en unidades de aprendizaje cursadas y aprobadas."
            : "Modalidad para docentes que brindan atención y orientación académica personalizada a los alumnos (cupo máximo de 10 tutorados).";

        public bool YaEsTutor { get; set; }
        public bool TieneSolicitudPendiente => UltimoEstado == EstadoSolicitud.Pendiente;
        public EstadoSolicitud? UltimoEstado { get; set; }
        public DateTime? FechaUltimaSolicitud { get; set; }
        public string? RespuestaCoordinacion { get; set; }
        public DateTime? FechaRespuesta { get; set; }

        public bool PuedeSolicitar => !YaEsTutor && !TieneSolicitudPendiente && PerfilCompleto;
    }
}
