using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace tutorias.Models
{
    public class TutorPerfil
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [StringLength(500)]
        public string? Biografia { get; set; }

        public bool EsPublico { get; set; } = false;

        public double ReputacionPromedio { get; set; } = 0.0;

        public ICollection<TutorMateria> MateriasImpartidas { get; set; } = new List<TutorMateria>();

        public ICollection<TutorHorario> Horarios { get; set; } = new List<TutorHorario>();
    }
}
