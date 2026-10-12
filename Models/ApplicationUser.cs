using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tutorias.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ApellidoPaterno { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ApellidoMaterno { get; set; } = string.Empty;

        [NotMapped]
        public string NombreCompleto => $"{Nombre} {ApellidoPaterno} {ApellidoMaterno}".Trim();

        [Required]
        [StringLength(20)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "La boleta solo debe contener dígitos numéricos.")]
        public string Boleta { get; set; } = string.Empty;

        public int? CarreraId { get; set; }
        public Carrera? Carrera { get; set; }

        [StringLength(150)]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$", ErrorMessage = "La academia solo puede contener letras y espacios.")]
        public string? Academia { get; set; }

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
