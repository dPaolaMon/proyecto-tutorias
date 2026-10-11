using System.ComponentModel.DataAnnotations;

namespace tutorias.Models
{
    public class Carrera
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Clave { get; set; } = string.Empty;

        public ICollection<Materia> Materias { get; set; } = new List<Materia>();
        public ICollection<ApplicationUser> Alumnos { get; set; } = new List<ApplicationUser>();
    }
}
