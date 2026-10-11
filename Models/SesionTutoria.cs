using System.ComponentModel.DataAnnotations;

namespace tutorias.Models
{
    public class SesionTutoria
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        public int MateriaId { get; set; }
        public Materia? Materia { get; set; }

        public string? TutorId { get; set; }
        public ApplicationUser? Tutor { get; set; }

        public TipoTutoria TipoTutoria { get; set; }

        public DateTime FechaHoraInicio { get; set; }
        public DateTime FechaHoraFin { get; set; }

        [StringLength(100)]
        public string Lugar { get; set; } = string.Empty;

        public int CapacidadMaxima { get; set; } = 10;

        public bool Activa { get; set; } = true;

        public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
    }
}
