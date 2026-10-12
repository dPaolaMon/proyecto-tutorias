using System.ComponentModel.DataAnnotations;

namespace tutorias.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
        [Display(Name = "Nombre(s)")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido paterno no puede exceder 100 caracteres.")]
        [Display(Name = "Apellido Paterno")]
        public string ApellidoPaterno { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido materno es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido materno no puede exceder 100 caracteres.")]
        [Display(Name = "Apellido Materno")]
        public string ApellidoMaterno { get; set; } = string.Empty;

        [Required(ErrorMessage = "La boleta es obligatoria.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "La boleta solo debe contener dígitos numéricos.")]
        [StringLength(20, ErrorMessage = "La boleta no puede exceder 20 dígitos.")]
        [Display(Name = "Boleta")]
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
        [Display(Name = "Carrera")]
        public int? CarreraId { get; set; }

        [StringLength(150, ErrorMessage = "La academia no puede exceder 150 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$", ErrorMessage = "La academia solo debe contener letras y espacios.")]
        [Display(Name = "Academia")]
        public string? Academia { get; set; }

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
        public string Nombre { get; set; } = string.Empty;
        public string ApellidoPaterno { get; set; } = string.Empty;
        public string ApellidoMaterno { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Boleta { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? CarreraNombre { get; set; }
        public string? Academia { get; set; }
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
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
        [Display(Name = "Nombre(s)")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido paterno no puede exceder 100 caracteres.")]
        [Display(Name = "Apellido Paterno")]
        public string ApellidoPaterno { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido materno es obligatorio.")]
        [StringLength(100, ErrorMessage = "El apellido materno no puede exceder 100 caracteres.")]
        [Display(Name = "Apellido Materno")]
        public string ApellidoMaterno { get; set; } = string.Empty;

        [Required(ErrorMessage = "La boleta es obligatoria.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "La boleta solo debe contener dígitos numéricos.")]
        [StringLength(20, ErrorMessage = "La boleta no puede exceder 20 dígitos.")]
        [Display(Name = "Boleta")]
        public string Boleta { get; set; } = string.Empty;

        // Inmutable: solo lectura informativa
        [Display(Name = "Correo Institucional (Inmutable)")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Carrera")]
        public int? CarreraId { get; set; }

        [StringLength(150, ErrorMessage = "La academia no puede exceder 150 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑüÜ\s]+$", ErrorMessage = "La academia solo debe contener letras y espacios.")]
        [Display(Name = "Academia")]
        public string? Academia { get; set; }

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
        [Display(Name = "Motivo de la baja (Opcional)")]
        [StringLength(500, ErrorMessage = "El motivo no puede exceder 500 caracteres.")]
        public string? MotivoBaja { get; set; }

        [Display(Name = "Confirmo que deseo dar de baja mi cuenta y desactivar mi perfil institucional.")]
        public bool ConfirmarBaja { get; set; }
    }
}
