using System.ComponentModel.DataAnnotations;

namespace tutorias.Models
{
    public class SolicitudTutor
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        public TipoTutor TipoTutorSolicitado { get; set; }

        public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;

        public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;

        [StringLength(255)]
        public string? RespuestaCoordinacion { get; set; }

        public DateTime? FechaRespuesta { get; set; }
    }
}
