using System.ComponentModel.DataAnnotations;

namespace tutorias.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La boleta o número de empleado es obligatorio.")]
        [Display(Name = "Boleta / Número de Empleado")]
        public string Boleta { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo institucional es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [Display(Name = "Correo Institucional")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo institucional es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [Display(Name = "Correo Institucional")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordarme en este dispositivo")]
        public bool RememberMe { get; set; }
    }

    public class CompleteProfileViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar una carrera o departamento.")]
        [Display(Name = "Carrera / Departamento")]
        public int CarreraId { get; set; }

        [Range(1, 12, ErrorMessage = "El semestre debe estar entre 1 y 12.")]
        [Display(Name = "Semestre")]
        public int? Semestre { get; set; }

        [Display(Name = "Fotografía de Perfil (Opcional)")]
        public IFormFile? Foto { get; set; }

        public bool EsDocente { get; set; }
    }

    public class ProfileViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Boleta { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? CarreraNombre { get; set; }
        public int? Semestre { get; set; }
        public string? FotoUrl { get; set; }
        public TipoTutor TipoTutor { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public bool PerfilCompleto { get; set; }
        public bool Activo { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string? MotivoBaja { get; set; }
        public bool EsDocente => Roles.Contains("Docente");
    }

    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres.")]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La boleta o número de empleado es obligatorio.")]
        [StringLength(20, ErrorMessage = "La boleta no puede exceder 20 caracteres.")]
        [Display(Name = "Boleta / Número de Empleado")]
        public string Boleta { get; set; } = string.Empty;

        // Inmutable: solo lectura informativa
        [Display(Name = "Correo Institucional (Inmutable)")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Carrera / Adscripción")]
        public int? CarreraId { get; set; }

        [Range(1, 12, ErrorMessage = "El semestre debe estar entre 1 y 12.")]
        [Display(Name = "Semestre actual")]
        public int? Semestre { get; set; }

        public string? FotoUrlActual { get; set; }

        [Display(Name = "Actualizar Fotografía (Opcional)")]
        public IFormFile? Foto { get; set; }

        public bool EsDocente { get; set; }
    }

    public class DeleteProfileViewModel
    {
        [Display(Name = "Motivo de la baja")]
        [StringLength(500, ErrorMessage = "El motivo no puede exceder 500 caracteres.")]
        public string? MotivoBaja { get; set; }

        [Required(ErrorMessage = "Debes confirmar que deseas dar de baja tu cuenta.")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Debes confirmar la baja para continuar.")]
        [Display(Name = "Confirmo que deseo dar de baja mi cuenta y desactivar mi perfil institucional.")]
        public bool ConfirmarBaja { get; set; }
    }
}
