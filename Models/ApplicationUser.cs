using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace tutorias.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(150)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Boleta { get; set; } = string.Empty;

        public int? CarreraId { get; set; }
        public Carrera? Carrera { get; set; }

        public int? Semestre { get; set; }

        [StringLength(255)]
        public string? FotoUrl { get; set; }

        public TipoTutor TipoTutor { get; set; } = TipoTutor.Ninguno;

        public bool PerfilCompleto { get; set; } = false;

        public bool Activo { get; set; } = true;

        public DateTime? FechaBaja { get; set; }

        [StringLength(500)]
        public string? MotivoBaja { get; set; }

        public TutorPerfil? PerfilTutor { get; set; }
    }
}
