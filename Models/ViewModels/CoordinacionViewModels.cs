using tutorias.Models;

namespace tutorias.Models.ViewModels
{
    public class GestionUsuariosViewModel
    {
        public string? Busqueda { get; set; }
        public string? RolFiltro { get; set; }
        public int TotalCoordinadores { get; set; }
        public int TotalUsuarios { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TamanoPagina { get; set; } = 10;
        public int TotalRegistros { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / (TamanoPagina > 0 ? TamanoPagina : 10));
        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
        public List<UsuarioCoordinacionItemViewModel> Usuarios { get; set; } = new();
    }

    public class UsuarioCoordinacionItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Boleta { get; set; } = string.Empty;
        public string? CarreraNombre { get; set; }
        public string? Academia { get; set; }
        public int? Semestre { get; set; }
        public string? FotoUrl { get; set; }
        public TipoTutor TipoTutor { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public bool EsCoordinador { get; set; }
        public bool EsUsuarioActual { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool Activo { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string? MotivoBaja { get; set; }

        public string InformacionAcademica => !string.IsNullOrWhiteSpace(Academia)
            ? $"Academia: {Academia}"
            : (!string.IsNullOrWhiteSpace(CarreraNombre) ? $"{CarreraNombre} (Semestre {Semestre?.ToString() ?? "N/A"})" : "Sin asignar");
    }

    public class BandejaSolicitudesViewModel
    {
        public string? Busqueda { get; set; }
        public string? RolFiltro { get; set; }
        public EstadoSolicitud? EstadoFiltro { get; set; }
        public int TotalPendientes { get; set; }
        public int TotalAprobadas { get; set; }
        public int TotalRechazadas { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TamanoPagina { get; set; } = 10;
        public int TotalRegistros { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / (TamanoPagina > 0 ? TamanoPagina : 10));
        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
        public List<SolicitudTutorItemViewModel> Solicitudes { get; set; } = new();
    }

    public class SolicitudTutorItemViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Boleta { get; set; } = string.Empty;
        public string ProgramaOAcademia { get; set; } = string.Empty;
        public bool EsDocente { get; set; }
        public TipoTutor TipoTutorSolicitado { get; set; }
        public string ModalidadDescripcion => TipoTutorSolicitado == TipoTutor.AlumnoTutor
            ? "Tutoría entre Pares"
            : "Tutoría Individual";
        public DateTime FechaSolicitud { get; set; }
        public EstadoSolicitud Estado { get; set; }
        public string? RespuestaCoordinacion { get; set; }
        public DateTime? FechaRespuesta { get; set; }
        public bool EsTutorActivo { get; set; }
    }
}
