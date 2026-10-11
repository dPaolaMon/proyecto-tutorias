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

        [StringLength(50)]
        public string? SalonAsignado { get; set; }
    }
}
