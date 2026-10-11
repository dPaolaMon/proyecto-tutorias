using System.ComponentModel.DataAnnotations;

namespace tutorias.Models
{
    public class Materia
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Clave { get; set; } = string.Empty;

        public int CarreraId { get; set; }
        public Carrera? Carrera { get; set; }
    }
}
