using System;
using System.ComponentModel.DataAnnotations;

namespace tutorias.Models
{
    public class TutorMateria
    {
        public int Id { get; set; }

        public int TutorPerfilId { get; set; }
        public TutorPerfil? TutorPerfil { get; set; }

        public int MateriaId { get; set; }
        public Materia? Materia { get; set; }

        public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;
    }
}
