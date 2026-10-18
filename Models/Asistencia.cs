using System.ComponentModel.DataAnnotations;

namespace tutorias.Models
{
    public class Asistencia
    {
        public int Id { get; set; }

        public int SesionId { get; set; }
        public SesionTutoria? Sesion { get; set; }

        [Required]
        public string AlumnoId { get; set; } = string.Empty;
        public ApplicationUser? Alumno { get; set; }

        public DateTime FechaHoraRegistro { get; set; } = DateTime.UtcNow;

        public MetodoAsistencia Metodo { get; set; }

        public bool Confirmada { get; set; } = false;

        public DateTime? FechaHoraConfirmacion { get; set; }

        [Range(1, 5)]
        public int? CalificacionEncuesta { get; set; }

        [StringLength(300)]
        public string? ComentarioEncuesta { get; set; }
    }
}
