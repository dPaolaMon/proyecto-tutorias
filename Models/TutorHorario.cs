using System;
using System.ComponentModel.DataAnnotations;

namespace tutorias.Models
{
    public enum DiaSemana
    {
        Lunes = 1,
        Martes = 2,
        Miercoles = 3,
        Jueves = 4,
        Viernes = 5
    }

    public class TutorHorario
    {
        public int Id { get; set; }

        public int TutorPerfilId { get; set; }
        public TutorPerfil? TutorPerfil { get; set; }

        public DiaSemana DiaSemana { get; set; }

        public TimeSpan HoraInicio { get; set; }

        public TimeSpan HoraFin { get; set; }

        [Required]
        [StringLength(100)]
        public string Lugar { get; set; } = string.Empty;

        public int? MateriaId { get; set; }
        public Materia? Materia { get; set; }

        public bool Activo { get; set; } = true;
    }
}
