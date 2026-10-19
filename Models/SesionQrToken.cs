using System;
using System.ComponentModel.DataAnnotations;

namespace tutorias.Models
{
    public class SesionQrToken
    {
        public int Id { get; set; }

        public int SesionId { get; set; }
        public SesionTutoria? Sesion { get; set; }

        [Required]
        [StringLength(100)]
        public string Token { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime FechaExpiracion { get; set; }

        public bool Activo { get; set; } = true;
    }
}
